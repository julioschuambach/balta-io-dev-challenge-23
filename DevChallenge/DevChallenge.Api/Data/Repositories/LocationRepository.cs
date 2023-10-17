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

        public async Task<IEnumerable<Location>> GetAllLocations()
        {
            var locations = await _context.Locations
                                          .AsNoTracking()
                                          .OrderBy(x => x.City)
                                          .ToListAsync();

            return locations;
        }

        public async Task<Location?> GetLocationById(string id)
        {
            var location = await _context.Locations
                                         .AsNoTracking()
                                         .FirstOrDefaultAsync(x => x.Id == id);

            return location;
        }

        public async Task<IEnumerable<Location>> GetLocationsByCity(string city)
        {
            var locations = await _context.Locations
                                          .AsNoTracking()
                                          .Where(x => x.City.ToLower() == city.ToLower())
                                          .OrderBy(x => x.State)
                                          .ToListAsync();

            return locations;
        }

        public async Task<IEnumerable<Location>> GetLocationsByState(string state)
        {
            var locations = await _context.Locations
                                          .AsNoTracking()
                                          .Where(x => x.State.ToLower() == state.ToLower())
                                          .OrderBy(x => x.City)
                                          .ToListAsync();

            return locations;
        }

        public async Task CreateLocation(Location location)
        {
            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();
        }

        public async Task<Location?> UpdateLocation(string id, LocationViewModel viewModel)
        {
            var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id);

            if (location == null)
                return null;

            location.Update(viewModel);
            _context.Locations.Update(location);
            await _context.SaveChangesAsync();

            return location;
        }

        public async Task<Location?> DeleteLocation(string id)
        {
            var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id);

            if (location == null) 
                return null;

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

            return location;
        }
    }
}
