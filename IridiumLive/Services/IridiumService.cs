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

namespace IridiumLive.Services
{
    
    ///Provide an easy way to create a new database context in any child class through the protected Options member.
    public class IridiumService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly DbContextOptionsBuilder<IridiumLiveDbContext> _optionsBuilder;

        public IridiumService(IConfiguration configuration)
        {
            _configuration = configuration;
            _optionsBuilder = new DbContextOptionsBuilder<IridiumLiveDbContext>();
            _connectionString = _configuration.GetConnectionString("Sqlite");
            _optionsBuilder.UseSqlite(_connectionString);
        }

        protected DbContextOptions<IridiumLiveDbContext> Options => _optionsBuilder.Options;
    }
}
