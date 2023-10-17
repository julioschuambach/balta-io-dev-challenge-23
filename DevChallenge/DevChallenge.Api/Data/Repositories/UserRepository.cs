using DevChallenge.Api.Data.Interfaces.Repositories;
using DevChallenge.Api.Models;
using DevChallenge.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DevChallenge.Api.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DevChallengeDbContext _context;

        public UserRepository(DevChallengeDbContext context)
        {
            _context = context;
        }

        public async Task CreateUser(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            var user = await _context.Users
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(x => x.Email == email);

            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var users = await _context.Users
                                      .AsNoTracking()
                                      .OrderBy(x => x.Email)
                                      .ToListAsync();

            return users;
        }
    }
}
