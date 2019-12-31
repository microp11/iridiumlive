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
 * This was a lot of work, in the end the only solution I was able to implement is this one below:
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
        public Task<ICollection<ViewIra>> GetLiveIraAsync(long utcTicks);
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

        /// <summary>
        /// If the utcTicks is zero, returns the last point. This fixes an issue that has to do with time ang gr-iridium.
        /// </summary>
        /// <param name="utcTicks"></param>
        /// <returns></returns>
        public async Task<ICollection<ViewIra>> GetLiveIraAsync(long utcTicks)
        {
            //TODO replace with view and write this properly
            using IridiumLiveDbContext _context = new IridiumLiveDbContext(_optionsBuilder.Options);
            FormattableString sqlString;
            if (utcTicks == 0)
            {
                sqlString = $@"
                    select i.Id, i.Time, i.UtcTicks, i.Quality, i.SatNo, s.Name, i.Beam, i.Lat, i.Lon, i.Alt
                    from Iras i
                    inner join Sats s on i.SatNo = s.SatNo
                    order by i.UtcTicks desc
                    limit 1";
                
                return await _context.ViewIras
                    .FromSqlInterpolated(sqlString)
                    .AsNoTracking()
                    .ToListAsync();
            }
            else
            {
                sqlString = $@"
                select i.Id, i.Time, i.UtcTicks, i.Quality, i.SatNo, s.Name, i.Beam, i.Lat, i.Lon, i.Alt
                from Iras i
                inner join Sats s on i.SatNo = s.SatNo
                order by i.UtcTicks"; 
                
                return await _context.ViewIras
                    .FromSqlInterpolated(sqlString)
                    .Where(s => s.UtcTicks > utcTicks)
                    .AsNoTracking()
                    .ToListAsync();
            }
        }
    }
}
