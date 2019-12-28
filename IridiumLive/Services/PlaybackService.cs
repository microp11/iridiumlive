using IridiumLive.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IridiumLive.Services
{
    public interface IPlaybackService
    {
        public Task<ICollection<ViewIra>> GetViewIraAsync(DateTime from, DateTime to);
    }
    
    public class PlaybackService : IPlaybackService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        readonly DbContextOptionsBuilder<IridiumLiveDbContext> _optionsBuilder;

        public PlaybackService(IConfiguration configuration)
        {
            _configuration = configuration;
            _optionsBuilder = new DbContextOptionsBuilder<IridiumLiveDbContext>();
            _connectionString = _configuration.GetConnectionString("Sqlite");
            _optionsBuilder.UseSqlite(_connectionString);
        }

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
            using IridiumLiveDbContext _context = new IridiumLiveDbContext(_optionsBuilder.Options);
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
