using Chinook.Models;

namespace Chinook.Services.Interfaces
{
    public interface IArtistService
    {
        Task<Artist> GetArtistById(long id);

        Task<List<Artist>> GetArtists();
    }
}
