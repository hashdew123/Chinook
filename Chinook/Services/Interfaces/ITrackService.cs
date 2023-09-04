using Chinook.ClientModels;

namespace Chinook.Services.Interfaces
{
    public interface ITrackService
    {
        Task<List<PlaylistTrack>> GetTracksByArtistId(long id, string customerId);
    }
}
