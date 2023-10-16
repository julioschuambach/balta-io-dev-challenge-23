using DevChallenge.Api.Models;

namespace DevChallenge.Api.Data.Interfaces.Repositories
{
    public interface IUserRepository
    {
        void CreateUser(User user);
        User? GetUserByUsername(string username);
        IEnumerable<User> GetAllUsers();
    }
}
