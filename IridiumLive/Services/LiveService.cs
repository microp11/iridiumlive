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

        public Task<long> GetLastUtcTicks();
    }

    public class LiveService : IridiumService, ILiveService
    {
        private readonly Stopwatch sw;

        public LiveService(IConfiguration configuration) : base(configuration)
        {
            sw = new Stopwatch();
        }

        public async Task<long> GetLastUtcTicks()
        {
            sw.Reset();
            sw.Start();

            long result = 0;

            using IridiumLiveDbContext _context = new IridiumLiveDbContext(Options);
            try
            {
                var a = await _context.Packets.OrderByDescending(x => x.UtcTicks).FirstOrDefaultAsync();
                if (a != null) result = a.UtcTicks;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetLastUtcTicks failed: {ex.Message}");
            }
            sw.Stop();
            Console.WriteLine($"GetLastUtcTicks in: {sw.ElapsedMilliseconds} ms.");
            return result;
        }

        public async Task<ICollection<ViewIra>> GetLiveIraAsync(long utcTicks)
        {
            sw.Reset();
            sw.Start();

            using IridiumLiveDbContext _context = new IridiumLiveDbContext(Options);
            var result = await _context.ViewIras
                .OrderByDescending(s => s.UtcTicks)
                .Where(s => s.UtcTicks > utcTicks)
                .AsNoTracking()
                .ToListAsync();
            sw.Stop();
            Console.WriteLine("GetLiveIraAsync in: {0} ms.", sw.ElapsedMilliseconds);
            return result;
        }
    }
}
