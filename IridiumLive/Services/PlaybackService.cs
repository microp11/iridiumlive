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
using System.Linq;
using System.Diagnostics;

namespace IridiumLive.Services
{
    public interface IPlaybackService
    {
        public Task<ICollection<ViewIra>> GetViewIraAsync(DateTime from, DateTime to);
    }
    
    public class PlaybackService : IridiumService, IPlaybackService
    {
        private readonly Stopwatch sw;
        public PlaybackService(IConfiguration configuration) : base(configuration)
        {
            sw = new Stopwatch();

        }

        public async Task<ICollection<ViewIra>> GetViewIraAsync(DateTime from, DateTime to)
        {
            sw.Reset();
            sw.Start();

            //this should be changed, no need for these conversions
            from = DateTime.SpecifyKind(from, DateTimeKind.Local);
            DateTimeOffset fromOffset = new DateTimeOffset(from);
            long fromUtcTicks = fromOffset.UtcTicks;

            to = DateTime.SpecifyKind(to, DateTimeKind.Local);
            DateTimeOffset toOffset = new DateTimeOffset(to);
            long toUtcTicks = toOffset.UtcTicks;

            using IridiumLiveDbContext _context = new IridiumLiveDbContext(Options);
            var x = await _context.ViewIras
                .Where(s => s.UtcTicks >= fromUtcTicks && s.UtcTicks <= toUtcTicks)
                .OrderBy(s => s.UtcTicks)
                .AsNoTracking()
                .ToListAsync();
            sw.Stop();
            Console.WriteLine("Playback result in: {0} ms.", sw.ElapsedMilliseconds);
            return x;

        }
    }
}
