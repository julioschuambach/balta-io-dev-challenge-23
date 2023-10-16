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

        public void CreateUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User? GetUserByUsername(string username)
        {
            var user = _context.Users
                               .AsNoTracking()
                               .FirstOrDefault(x => x.Username == username);

            return user;
        }

        public IEnumerable<User> GetAllUsers()
        {
            var users = _context.Users
                                .AsNoTracking()
                                .OrderBy(x => x.Username)
                                .ToList();

            return users;
        }
    }
}
