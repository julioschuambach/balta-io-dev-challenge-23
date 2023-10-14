namespace DevChallenge.Api.ViewModels
{
    public class IbgeViewModel
    {
        public string State { get; private set; }
        public string City { get; set; }

        public IbgeViewModel(string state, string city)
            => (State, City) = (state, city);
    }
}
