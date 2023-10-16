using DevChallenge.Api.Data.Interfaces.Repositories;
using DevChallenge.Api.ViewModels;
using DevChallenge.Data.Contexts;
using DevChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace DevChallenge.Api.Data.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly DevChallengeDbContext _context;

        public LocationRepository(DevChallengeDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Location> GetAllLocations()
        {
            var locations = _context.Locations
                                    .AsNoTracking()
                                    .OrderBy(x => x.City)
                                    .ToList();

            return locations;
        }

        public Location? GetLocationById(string id)
        {
            var location = _context.Locations
                                   .AsNoTracking()
                                   .FirstOrDefault(x => x.Id == id);

            return location;
        }

        public IEnumerable<Location> GetLocationsByCity(string city)
        {
            var locations = _context.Locations
                                    .AsNoTracking()
                                    .Where(x => x.City.ToLower() == city.ToLower())
                                    .OrderBy(x => x.State)
                                    .ToList();

            return locations;
        }

        public IEnumerable<Location> GetLocationsByState(string state)
        {
            var locations = _context.Locations
                                    .AsNoTracking()
                                    .Where(x => x.State.ToLower() == state.ToLower())
                                    .OrderBy(x => x.City)
                                    .ToList();

            return locations;
        }

        public void CreateLocation(Location location)
        {
            _context.Locations.Add(location);
            _context.SaveChanges();
        }

        public Location? UpdateLocation(string id, LocationViewModel viewModel)
        {
            var location = _context.Locations.FirstOrDefault(x => x.Id == id);

            if (location == null)
                return null;

            location.Update(viewModel);
            _context.Locations.Update(location);
            _context.SaveChanges();

            return location;
        }

        public Location? DeleteLocation(string id)
        {
            var location = _context.Locations.FirstOrDefault(x => x.Id == id);

            if (location == null) 
                return null;

            _context.Locations.Remove(location);
            _context.SaveChanges();

            return location;
        }
    }
}
