using DevChallenge.Api.Models;

namespace DevChallenge.Api.Data.Interfaces.Repositories
{
    public interface IUserRepository
    {
        void CreateUser(User user);
        User? GetUserByEmail(string email);
        IEnumerable<User> GetAllUsers();
    }
}
