using Chinook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Chinook.ClientModels;
using Chinook.Models;
using Playlist = Chinook.ClientModels.Playlist;
using EntityPlaylist = Chinook.Models.Playlist;

namespace Chinook.Services
{
    public class PlaylistService : IPlaylistService
    {
        private IDbContextFactory<ChinookContext> _dbFactory { get; set; }
        public PlaylistService(IDbContextFactory<ChinookContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<Playlist> GetPlaylistById(long playListId, string customerId)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            var playlist = dbContext.Playlists
                .Include(a => a.Tracks).ThenInclude(a => a.Album).ThenInclude(a => a.Artist)
                .Where(p => p.PlaylistId == playListId)
                .Select(p => new Playlist()
                {
                    Name = p.Name,
                    Tracks = p.Tracks.Select(t => new PlaylistTrack()
                    {
                        AlbumTitle = t.Album.Title,
                        ArtistName = t.Album.Artist.Name,
                        TrackId = t.TrackId,
                        TrackName = t.Name,
                        IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == customerId && up.Playlist.Name == "Favorites")).Any()
                    }).ToList()
                })
                .FirstOrDefault();

            return playlist;
        }

        public async Task AddToUserFavouritesByTrackId(long id, string customerId)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            var playlist = dbContext.Playlists.Include(t => t.UserPlaylists).Include(x => x.Tracks)
                .FirstOrDefault(item => item.Name == "Favorites" && item.UserPlaylists
                .Any(t => t.UserId == customerId));

            var track = await dbContext.Tracks.SingleAsync(item => item.TrackId == id);

            if (playlist != null && track != null)
            {
                playlist.Tracks.Add(track);

                dbContext.SaveChanges();
                return;
            }

            var newPlaylist = new EntityPlaylist
            {
                Name = "Favorites",
            };

            newPlaylist.Tracks.Add(track);

            var userPlaylist = new UserPlaylist
            {
                UserId = customerId,
                Playlist = newPlaylist
            };

            dbContext.UserPlaylists.Add(userPlaylist);

            dbContext.SaveChanges();
        }

        public async Task RemoveFromUserFavouritesByTrackId(long id, string customerId)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            var playlist = dbContext.Playlists.Include(t => t.UserPlaylists).Include(x => x.Tracks)
                .FirstOrDefault(item => item.UserPlaylists
                .Any(t => t.UserId == customerId) && item.UserPlaylists
                .Any(t => t.Playlist.Tracks.Any(y => y.TrackId == id)));

            if (playlist != null)
            {
                var track = playlist.Tracks.FirstOrDefault(x => x.TrackId == id);
                if (track != null)
                {
                    playlist.Tracks.Remove(track);
                    dbContext.SaveChanges();
                };
            }
        }

        public async Task<List<EntityPlaylist>> GetPlaylistsByUserId(string customerId)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            var playlists = dbContext.Playlists.Include(t => t.UserPlaylists)
                .Where(item => item.UserPlaylists.Any(t => t.UserId == customerId));

            return playlists.ToList();
        }

        public async Task AddTrackToExistingPlaylistById(long id, long playistId, string customerId)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            var playlist = dbContext.Playlists.Include(t => t.UserPlaylists).Include(x => x.Tracks)
                .FirstOrDefault(item => item.UserPlaylists
                .Any(t => t.UserId == customerId) && item.UserPlaylists
                .Any(t => t.Playlist.PlaylistId == playistId));

            if (playlist != null)
            {
                var track = await dbContext.Tracks.SingleAsync(item => item.TrackId == id);
                if (track != null)
                {
                    playlist.Tracks.Add(track);
                    dbContext.SaveChanges();
                };
            }
        }

        public async Task AddTrackToNewPlaylistById(long id, string playlistName, string customerId)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();
            var track = await dbContext.Tracks.SingleAsync(item => item.TrackId == id);

            var newPlaylist = new EntityPlaylist
            {
                Name = playlistName,
            };

            newPlaylist.Tracks.Add(track);

            var userPlaylist = new UserPlaylist
            {
                UserId = customerId,
                Playlist = newPlaylist
            };

            dbContext.UserPlaylists.Add(userPlaylist);

            dbContext.SaveChanges();
        }
    }
}
