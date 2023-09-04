using Chinook.Models;
using Chinook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class ArtistService : IArtistService
    {
        private IDbContextFactory<ChinookContext> _dbFactory { get; set; }
        public ArtistService(IDbContextFactory<ChinookContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<Artist> GetArtistById(long id)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            var artist = dbContext.Artists.SingleOrDefault(a => a.ArtistId == id);

            return artist ?? new Artist();
        }

        public async Task<List<Artist>> GetArtists()
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            var artists = dbContext.Artists.Include(i => i.Albums).ToList();

            return artists;
        }
    }
}
