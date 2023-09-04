using Chinook.ClientModels;
using EntityPlayList = Chinook.Models.Playlist;

namespace Chinook.Services.Interfaces
{
    public interface IPlaylistService
    {
        Task<List<EntityPlayList>> GetPlaylistsByUserId(string customerId);

        Task AddTrackToExistingPlaylistById(long id, long playistId, string customerId);

        Task AddTrackToNewPlaylistById(long id, string playlistName, string customerId);

        Task<Playlist> GetPlaylistById(long playListId, string customerId);

        Task AddToUserFavouritesByTrackId(long id, string customerId);

        Task RemoveFromUserFavouritesByTrackId(long id, string customerId);
    }
}
