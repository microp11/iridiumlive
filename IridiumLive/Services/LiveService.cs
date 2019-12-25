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
using System.Linq;
using System.Threading.Tasks;

namespace IridiumLive.Services
{
    public interface ILiveService
    {
        public Task<ICollection<LiveIra>> GetLiveIraAsync(long utcTicks);
    }

    public class LiveService : ILiveService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        readonly DbContextOptionsBuilder<IridiumLiveDbContext> _optionsBuilder;

        public LiveService(IConfiguration configuration)
        {
            _configuration = configuration;
            _optionsBuilder = new DbContextOptionsBuilder<IridiumLiveDbContext>();
            _connectionString = _configuration.GetConnectionString("Sqlite");
            _optionsBuilder.UseSqlite(_connectionString);
        }

        public async Task<ICollection<LiveIra>> GetLiveIraAsync(long utcTicks)
        {
            //TODO replace with view
            using IridiumLiveDbContext _context = new IridiumLiveDbContext(_optionsBuilder.Options);
            FormattableString sqlString = $@"
                select i.Id, i.Time, i.UtcTicks, i.Quality, i.SatNo, s.Name, i.Beam, i.Lat, i.Lon, i.Alt
                from Iras i
                inner join Sats s on i.SatNo = s.SatNo";
            return await _context.LiveIras.FromSqlInterpolated(sqlString).Where(s => s.UtcTicks > utcTicks).OrderBy(s => s.UtcTicks).AsNoTracking().ToListAsync();
        }
    }
}
