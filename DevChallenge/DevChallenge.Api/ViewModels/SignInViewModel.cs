namespace DevChallenge.Api.ViewModels
{
    public class SignInViewModel
    {
        public string Username { get; private set; }
        public string Password { get; private set; }

        public SignInViewModel(string username, string password)
            => (Username, Password) = (username, password);
    }
}
