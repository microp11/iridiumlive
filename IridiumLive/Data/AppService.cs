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
 *
 */

using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IridiumLive.Data
{
    public class AppService
    {
        private readonly ServiceDbContext _context;

        public AppService(ServiceDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddRxLine(string rxLine)
        {
            try
            {
                string[] words = rxLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string[] words1 = words[1].Split('-');
                DateTimeOffset satTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(words1[1])).AddMilliseconds(Convert.ToInt64(words[2])).ToLocalTime();
                long utcTicks = satTime.ToUniversalTime().UtcTicks;
                int quality = Convert.ToInt32(words[4].TrimEnd('%'));
                int satNo;
                //Debug.WriteLine("{0} {1}", words[0], satTime);

                if (words[0] == "IRA:")
                {
                    //Debug.WriteLine("{0} {1} {2}", words[0], satTime, utcTicks);

                    Ira ira = new Ira
                    {
                        Id = rxLine,

                        Time = satTime,

                        UtcTicks = utcTicks,

                        Quality = quality
                    };

                    //sat:26 -> [8]
                    satNo = Convert.ToInt32(words[8].Substring(4));
                    ira.SatNo = satNo;

                    //beam:44 -> [9]
                    ira.Beam = Convert.ToInt32(words[9].Substring(5));

                    //pos=(+51.18/-068.82) -> [10]
                    string[] words2 = words[10].Split('(', '/', ')');
                    ira.Lat = Convert.ToDouble(words2[1]);
                    ira.Lon = Convert.ToDouble(words2[2]);

                    //alt=796 -> [11]
                    ira.Alt = Convert.ToDouble(words[11].Substring(4));

                    _context.Iras.Add(ira);
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        return false;
                    }
                }
                else if (words[0] == "IBC:")
                {
                    Ibc ibc = new Ibc
                    {
                        Id = rxLine,

                        Time = satTime,

                        UtcTicks = utcTicks,

                        Quality = quality
                    };

                    //sat:26 -> [9]
                    satNo = Convert.ToInt32(words[9].Substring(4));
                    ibc.SatNo = satNo;

                    //cell:31 -> [10]
                    ibc.Cell = Convert.ToInt32(words[10].Substring(5));

                    _context.Ibcs.Add(ibc);
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                Sat sat = new Sat
                {
                    Id = Guid.NewGuid().ToString(),
                    SatNo = satNo,
                    Name = satNo.ToString()
                };

                if (!SatExists(satNo))
                {
                    _context.Sats.Add(sat);
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException ex)
                    {
                        Debug.WriteLine(ex.Message);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        private bool SatExists(int rxId)
        {
            return _context.Sats.Any(e => e.SatNo == rxId);
        }
    }
}
