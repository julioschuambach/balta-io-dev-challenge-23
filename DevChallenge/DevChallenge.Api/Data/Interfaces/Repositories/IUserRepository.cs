using DevChallenge.Api.Models;

namespace DevChallenge.Api.Data.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task CreateUser(User user);
        Task<User?> GetUserByEmail(string email);
        Task<IEnumerable<User>> GetAllUsers();
    }
}
