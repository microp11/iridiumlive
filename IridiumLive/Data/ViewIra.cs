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
 * Uses a view instead of a table: https://www.laboremus.ug/post/using-sql-views-with-entity-framework-code-first
 * 
 */

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IridiumLive.Data
{
    [Table("V_ViewIras")]
    public class ViewIra
    {
        [Key]
        public string Id { get; set; }
        public DateTimeOffset Time { get; set; }
        public long UtcTicks { get; set; }
        public int Quality { get; set; }
        public int SatNo { get; set; }        //sat:26 -> [8]
        public string Name { get; set; }
        public int Beam { get; set; }       //beam:44 => [9]
        public double Lat { get; set; }
        public double Lon { get; set; }     //pos=(+51.18/-068.82) -> [10]
        public double Alt { get; set; }     //alt=796 -> [11]
    }
}