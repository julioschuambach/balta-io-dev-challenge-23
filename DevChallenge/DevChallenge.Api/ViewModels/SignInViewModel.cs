namespace DevChallenge.Api.ViewModels
{
    public class SignInViewModel
    {
        public string Email { get; private set; }
        public string Password { get; private set; }

        public SignInViewModel(string email, string password)
            => (Email, Password) = (email, password);
    }
}
