/*
 * microp11 2019
 * 
 * This file is part of IridiumLive.
 * 
 * IridiumLive is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * IridiumLive is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with IridiumLive.  If not, see <http://www.gnu.org/licenses/>.
 *
 * This was a lot of work, in the end the only solution i was able to implement is this one below:
 * https://stackoverflow.com/questions/38417051/what-goes-into-dbcontextoptions-when-invoking-a-new-dbcontext
 * 
 * I have tried these ones as well, unsuccessfully:
 * https://stackoverflow.com/questions/58346573/blazor-a-second-operation-started-on-this-context-before-a-previous-operation-c/58347502#58347502
 * https://github.com/aspnet/AspNetCore/issues/10448
 * https://stackoverflow.com/questions/58346573/blazor-a-second-operation-started-on-this-context-before-a-previous-operation-c
 *
 */

using IridiumLive.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IridiumLive.Services
{
    public interface IStatsService
    {
        public Task<ICollection<Stat>> GetStatsAsync();
    }

    public class StatsService : IStatsService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        readonly DbContextOptionsBuilder<IridiumLiveDbContext> _optionsBuilder;

        public StatsService(IConfiguration configuration)
        {
            _configuration = configuration;
            _optionsBuilder = new DbContextOptionsBuilder<IridiumLiveDbContext>();
            _connectionString = _configuration.GetConnectionString("Sqlite");
            _optionsBuilder.UseSqlite(_connectionString);
        }

        public async Task<ICollection<Stat>> GetStatsAsync()
        {
            using IridiumLiveDbContext _context = new IridiumLiveDbContext(_optionsBuilder.Options);
            FormattableString sqlString = $@"
                select s.SatNo, IFNUll(x.Count, 0) Iras, IFNULL(y.Count, 0) Ibcs 
                from Sats s
                left outer join(select a.SatNo, count(*) Count from Iras a group by a.SatNo) x on s.SatNo = x.SatNo
                left outer join(select b.SatNo, count(*) Count from Ibcs b group by b.SatNo) y on s.SatNo = y.SatNo
                order by s.SatNo";
            var l = await _context.Stats.FromSqlInterpolated(sqlString).AsNoTracking().ToListAsync();
            return l;
        }
    }
}
