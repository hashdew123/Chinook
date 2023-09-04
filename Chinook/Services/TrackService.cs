using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class TrackService : ITrackService
    {
        private IDbContextFactory<ChinookContext> _dbFactory { get; set; }
        public TrackService(IDbContextFactory<ChinookContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public async Task<List<PlaylistTrack>> GetTracksByArtistId(long id, string customerId)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            var tracks = dbContext.Tracks.Where(a => a.Album.ArtistId == id)
            .Include(a => a.Album)
            .Select(t => new PlaylistTrack()
            {
                AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                TrackId = t.TrackId,
                TrackName = t.Name,
                IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == customerId && up.Playlist.Name == "Favorites")).Any()
            })
            .ToList();

            return tracks;
        }
    }
}
