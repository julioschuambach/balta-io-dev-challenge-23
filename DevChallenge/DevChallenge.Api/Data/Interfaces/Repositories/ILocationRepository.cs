using DevChallenge.Api.ViewModels;
using DevChallenge.Models;

namespace DevChallenge.Api.Data.Interfaces.Repositories
{
    public interface ILocationRepository
    {
        IEnumerable<Location> GetAllLocations();
        Location? GetLocationById(string id);
        IEnumerable<Location> GetLocationsByCity(string city);
        IEnumerable<Location> GetLocationsByState(string state);
        void CreateLocation(Location location);
        Location? UpdateLocation(string id, LocationViewModel viewModel);
        Location? DeleteLocation(string id);
    }
}
