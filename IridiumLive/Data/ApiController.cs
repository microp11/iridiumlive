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

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IridiumLive.Data
{
    [ApiController]
    public class ApiController : Controller
    {
        private readonly ServiceDbContext _context;

        public ApiController(ServiceDbContext context)
        {
            _context = context;
        }

        // GET: api/Sats
        [HttpGet("api/sats")]
        public async Task<ActionResult<IEnumerable<Sat>>> GetSats()
        {
            return await _context.Sats.OrderBy(s => s.SatNo).ToListAsync();
        }

        // GET: api/Sats/5
        [HttpGet("api/sats/{id}")]
        public async Task<ActionResult<Sat>> GetSat(string id)
        {
            var sat = await _context.Sats.FindAsync(id);

            if (sat == null)
            {
                return NotFound();
            }

            return sat;
        }

        // PUT: api/Sats/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("api/sats/{id}")]
        public async Task<IActionResult> PutSat(string id, Sat sat)
        {
            if (id != sat.Id)
            {
                return BadRequest();
            }

            _context.Entry(sat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SatExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Sats
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost("api/sats")]
        public async Task<ActionResult<Sat>> PostSat(Sat sat)
        {
            _context.Sats.Add(sat);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SatExists(sat.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSat", new { id = sat.Id }, sat);
        }

        // DELETE: api/Sats/5
        [HttpDelete("api/sats/{id}")]
        public async Task<ActionResult<Sat>> DeleteSat(string id)
        {
            var sat = await _context.Sats.FindAsync(id);
            if (sat == null)
            {
                return NotFound();
            }

            _context.Sats.Remove(sat);
            await _context.SaveChangesAsync();

            return sat;
        }

        private bool SatExists(string id)
        {
            return _context.Sats.Any(e => e.Id == id);
        }

        [HttpDelete]
        [Route("api/deletedb")]
        public bool DeleteDbConfirmed()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            return true;
        }

        // GET: api/Stats
        [HttpGet("api/stats")]
        public async Task<ActionResult<IEnumerable<Stat>>> GetStats()
        {
                FormattableString sqlString = $"select x.SatNo, IFNUll(x.Count, 0) Iras, IFNULL(y.Count, 0) Ibcs from (select a.SatNo, count(*) Count from Iras a group by a.SatNo) x left outer join (select a.SatNo, count(*) Count from Ibcs a group by a.SatNo) y on x.SatNo = y.SatNo order by x.SatNo";
                return await _context.Stats.FromSqlInterpolated(sqlString).OrderBy(s => s.SatNo).ToListAsync();
        }

        // GET: api/View/Ira
        [HttpGet("api/view/ira")]
        public async Task<ActionResult<IEnumerable<LiveIra>>> GetView()
        {
            long utcTicks = Convert.ToInt64(HttpContext.Request.Query["utcTicks"]);
            //return await _context.Iras.Where(s => s.UtcTicks > utcTicks).OrderBy(s => s.UtcTicks).ToListAsync();
            FormattableString sqlString = $"select i.Id, i.Time, i.UtcTicks, i.Quality, i.SatNo, s.Name, i.Beam, i.Lat, i.Lon, i.Alt from Iras i inner join Sats s on i.SatNo = s.SatNo";
            return await _context.LiveIras.FromSqlInterpolated(sqlString).Where(s => s.UtcTicks > utcTicks).OrderBy(s => s.UtcTicks).ToListAsync();
        }
    }
}
