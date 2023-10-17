using DevChallenge.Api.ViewModels;
using DevChallenge.Models;

namespace DevChallenge.Api.Data.Interfaces.Repositories
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Location>> GetAllLocations();
        Task<Location?> GetLocationById(string id);
        Task<IEnumerable<Location>> GetLocationsByCity(string city);
        Task<IEnumerable<Location>> GetLocationsByState(string state);
        Task CreateLocation(Location location);
        Task<Location?> UpdateLocation(string id, LocationViewModel viewModel);
        Task<Location?> DeleteLocation(string id);
    }
}
