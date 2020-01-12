/*
 * microp11 2020
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
    public interface IPlaybackService
    {
        public Task<ICollection<ViewIra>> GetViewIraAsync(DateTime from, DateTime to);
    }
    
    public class PlaybackService : IridiumService, IPlaybackService
    {
        public PlaybackService(IConfiguration configuration) : base(configuration) { }

        public async Task<ICollection<ViewIra>> GetViewIraAsync(DateTime from, DateTime to)
        {
            //this should be changed fundamentally, no need for these conversions
            from = DateTime.SpecifyKind(from, DateTimeKind.Local);
            DateTimeOffset fromOffset = new DateTimeOffset(from);
            long fromUtcTicks = fromOffset.UtcTicks;

            to = DateTime.SpecifyKind(to, DateTimeKind.Local);
            DateTimeOffset toOffset = new DateTimeOffset(to);
            long toUtcTicks = toOffset.UtcTicks;

            //TODO replace with view
            using IridiumLiveDbContext _context = new IridiumLiveDbContext(Options);
            FormattableString sqlString = $@"
                select i.Id, i.Time, i.UtcTicks, i.Quality, i.SatNo, s.Name, i.Beam, i.Lat, i.Lon, i.Alt
                from Iras i
                inner join Sats s on i.SatNo = s.SatNo
                where i.Utcticks >= {fromUtcTicks} and i.UtcTicks <= {toUtcTicks}
                order by i.UtcTicks";
            return await _context.ViewIras
                .FromSqlInterpolated(sqlString)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
