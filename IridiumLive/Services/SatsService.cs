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

using IridiumLive.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace IridiumLive.Services
{
    public interface ISatsService
    {
        public Task<ICollection<Sat>> GetSatsAsync();
        
        public Task<Sat> GetSatAsync(string id);

        public Task<bool> PostSatAsync(Sat sat);

        public Task<bool> PutSatAsync(string id, Sat sat);

        public Task<bool> AddRxLineAsync(string rxLine);
    }

    public class SatsService : IridiumService, ISatsService
    {
        public SatsService(IConfiguration configuration) : base(configuration)
        {
            //Console.WriteLine("Only IRA gets charted.");
        }

        public async Task<ICollection<Sat>> GetSatsAsync()
        {
            using IridiumLiveDbContext _context = new IridiumLiveDbContext(Options); 
            return await _context.Sats.OrderBy(s => s.SatNo).AsNoTracking().ToListAsync();
        }

        public async Task<Sat> GetSatAsync(string id)
        {
            using IridiumLiveDbContext _context = new IridiumLiveDbContext(Options); 
            return await _context.Sats.FindAsync(id);
        }

        public async Task<bool> PostSatAsync(Sat sat)
        {
            using IridiumLiveDbContext _context = new IridiumLiveDbContext(Options); 
            _context.Sats.Add(sat);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SatExists(sat.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }

        public async Task<bool> PutSatAsync(string id, Sat sat)
        {
            if (id != sat.Id)
            {
                return false;
            }

            using IridiumLiveDbContext _context = new IridiumLiveDbContext(Options);
            _context.Entry(sat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SatExists(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }

        /// <summary>
        /// Change in format of data coming out of gr-iridium as based on 
        /// the gnuradio 3.7 to gnuradio 3.8
        /// 3.7+
        /// I do not have that version anymore :(
        /// 
        /// 3.8+
        /// 0    1         2              3           4     5     6   7  8       9       10                      11                   12      13     14  15
        /// IRA: u-rtl.sdr 014775683.5648 1626237568  95%   0.004 095 DL sat:081 beam:38 xyz=(+1043,-1296,+0658) pos=(+21.58/-051.17) alt=800 RAI:48 ?00 bc_sb:21
        /// IRA: u-rtl.sdr 014780003.6543 1626237440  91%   0.003 098 DL sat:081 beam:38 xyz=(+0718,-1110,+0889) pos=(+33.92/-057.10) alt=017 RAI:48 ?00 bc_sb:21
        /// 
        /// ITL: u-rtl.sdr 014785220.6331 1626082944  96%   0.004 432 DL<11>[45.77.f8.ef.53.fd.cc.51.ed.ba.67.35.e8.95.b2.1c.00.32.a3.f0.55.31.c8.f1.65.b1.ef.d7.3f.fd.4d.c2][57.2c.be.d3.4d.d2.53.d7.e1.e1.91.28.15.0f.58.f1.d7.09.6f.01.86.05.1e.94.aa.d9.26.ce.aa.f1.ea.9d][e4.5e.ff.11.eb.45.77.6b.f0.50.4d.f3.ae.11.b5.c2.79.ca.aa.4f.95.45.4a.d3.9d.e5.12.c2.bc.ec.16.a0]
        /// 
        /// 0    1         2              3           4     5     6   7  8    9       10     11 12     13        14                     15       16      17 18   19                                  20 21
        /// IBC: u-rtl.sdr 031321643.0324 1624882560  95%   0.004 136 DL bc:0 sat:025 cell:24 0 slot:0 sv_blkn:0 aq_cl:1111111111111111 aq_sb:27 aq_ch:2 00 0000 tmsi_expiry:2020-06-25T14:18:30.44Z [] []
        /// IBC: u-rtl.sdr 031321643.0324 1624882560  95%   0.004 136 DL bc:0 sat:025 cell:24 0 slot:0 sv_blkn:0 aq_cl:1111111111111111 aq_sb:27 aq_ch:2 00 0000 tmsi_expiry:2020-06-25T14:18:30.44Z [] []
        /// 
        /// Included fix by: @ereinitzhuber as committed to ereinitzhuber/iridiumlive
        /// </summary>
        /// <param name="rxLine"></param>
        /// <returns></returns>
        public async Task<bool> AddRxLineAsync(string rxLine)
        {
            using IridiumLiveDbContext _context = new IridiumLiveDbContext(Options); 
            try
            {
                string newGuid = Guid.NewGuid().ToString();
                string[] words = rxLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                long currentstamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                DateTimeOffset satTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(currentstamp, CultureInfo.InvariantCulture)).ToLocalTime();
                long utcTicks = satTime.ToUniversalTime().UtcTicks;
                int quality = Convert.ToInt32(words[4].TrimEnd('%'), CultureInfo.InvariantCulture);
                int satNo;
                //Debug.WriteLine(rxLine);
                //Debug.WriteLine("{0} {1}", words[0], satTime);

                //store everything
                Packet packets = new Packet
                {
                    Id = newGuid,
                    Time = satTime,
                    UtcTicks = utcTicks,
                    Quality = quality,
                    PacketId = words[0][..3]
                };

                _context.Packets.Add(packets);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    return false;
                }

                //store for immediate view
                if (words[0] == "IRA:")
                {
                    Console.WriteLine("{0} {1} {2}", words[0], satTime, utcTicks);

                    Ira ira = new Ira
                    {
                        Id = newGuid,
                        Time = satTime,
                        UtcTicks = utcTicks,
                        Quality = quality
                    };

                    //sat:26 -> [8]
                    satNo = Convert.ToInt32(words[8][4..], CultureInfo.InvariantCulture);
                    ira.SatNo = satNo;

                    //beam:44 -> [9]
                    ira.Beam = Convert.ToInt32(words[9][5..], CultureInfo.InvariantCulture);

                    //pos=(+51.18/-068.82) -> [11] changed from gr-iridium gnuradio 3.7 to 3.8
                    string[] words2 = words[11].Split('(', '/', ')');
                    ira.Lat = Convert.ToDouble(words2[1], CultureInfo.InvariantCulture);
                    ira.Lon = Convert.ToDouble(words2[2], CultureInfo.InvariantCulture);

                    //alt=796 -> [12] changed from gr-iridium gnuradio 3.7 to 3.8
                    ira.Alt = Convert.ToDouble(words[12][4..], CultureInfo.InvariantCulture);

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
                    //Console.WriteLine("{0} {1} {2}", words[0], satTime, utcTicks); 
                    Ibc ibc = new Ibc
                    {
                        Id = newGuid,
                        Time = satTime,
                        UtcTicks = utcTicks,
                        Quality = quality
                    };

                    //sat:26 -> [9]
                    satNo = Convert.ToInt32(words[9][4..], CultureInfo.InvariantCulture);
                    ibc.SatNo = satNo;

                    //cell:31 -> [10]
                    ibc.Cell = Convert.ToInt32(words[10][5..], CultureInfo.InvariantCulture);

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
                    //no IRA/IBC so no sat number present
                    return false;
                }

                //store IRA/IBC into the sat table, they have sat numbers
                //store to avoid query, which could be a compounded view
                //as it will save on write operations to db, however the view might be more expensive
                //need to research
                if (!SatExists(satNo))
                {
                    Sat sat = new Sat
                    {
                        //it does not matter we use an existing Guid as long as it is unique
                        Id = newGuid,
                        SatNo = satNo,
                        Name = satNo.ToString()
                    };

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

        private bool SatExists(string id)
        {
            using IridiumLiveDbContext _context = new IridiumLiveDbContext(Options); 
            return _context.Sats.Any(e => e.Id == id);
        }

        private bool SatExists(int rxId)
        {
            using IridiumLiveDbContext _context = new IridiumLiveDbContext(Options); 
            return _context.Sats.Any(e => e.SatNo == rxId);
        }
    }
}
