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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IridiumLive.Services
{
    public interface ILiveService
    {
        public Task<ICollection<ViewIra>> GetLiveIraAsync(long utcTicks);
    }

    public class LiveService : IridiumService, ILiveService
    {
        private readonly Stopwatch sw;

        public LiveService(IConfiguration configuration) : base(configuration)
        {
            sw = new Stopwatch();
        }

        /// <summary>
        /// If the utcTicks is zero, returns the most recent record.
        /// The returned records are in descending order. Important!
        /// </summary>
        /// <param name="utcTicks"></param>
        /// <returns></returns>
        public async Task<ICollection<ViewIra>> GetLiveIraAsync(long utcTicks)
        {
            sw.Reset();
            sw.Start();

            using IridiumLiveDbContext _context = new IridiumLiveDbContext(Options);
            if (utcTicks == 0)
            {
                var result0 = await _context.ViewIras
                    .OrderByDescending(s => s.UtcTicks)
                    .Take(1)
                    .AsNoTracking()
                    .ToListAsync();
                sw.Stop();
                Console.WriteLine("Live result in: {0} ms.", sw.ElapsedMilliseconds);
                return result0;
            }
            else
            {
                var result = await _context.ViewIras
                    .OrderByDescending(s => s.UtcTicks)
                    .Where(s => s.UtcTicks > utcTicks)
                    .AsNoTracking()
                    .ToListAsync();
                sw.Stop();
                Console.WriteLine("Live result in: {0} ms.", sw.ElapsedMilliseconds);
                return result;
            }
        }
    }
}
