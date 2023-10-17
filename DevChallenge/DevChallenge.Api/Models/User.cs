using DevChallenge.Api.ViewModels;

namespace DevChallenge.Api.Models
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string Role { get; private set; }

        public User() { }

        public User(SignUpViewModel viewModel)
        {
            Id = Guid.NewGuid();
            Email = viewModel.Email;
            Password = viewModel.Password;
            Role = "user";
        }
    }
}
