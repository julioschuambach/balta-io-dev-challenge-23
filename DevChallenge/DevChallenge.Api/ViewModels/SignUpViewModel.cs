namespace DevChallenge.Api.ViewModels
{
    public class SignUpViewModel
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string ConfirmPassword { get; private set; }

        public SignUpViewModel(string username, string password, string confirmPassword)
            => (Username, Password, ConfirmPassword) = (username, password, confirmPassword);
    }
}
