using DevChallenge.Api.ViewModels;

namespace DevChallenge.Api.Models
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Role { get; private set; }

        public User() { }

        public User(SignUpViewModel viewModel)
        {
            Id = Guid.NewGuid();
            Username = viewModel.Username;
            Password = viewModel.Password;
            Role = "user";
        }
    }
}
