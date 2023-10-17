namespace DevChallenge.Api.ViewModels
{
    public class SignUpViewModel
    {
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string ConfirmPassword { get; private set; }

        public SignUpViewModel(string email, string password, string confirmPassword)
            => (Email, Password, ConfirmPassword) = (email, password, confirmPassword);
    }
}
