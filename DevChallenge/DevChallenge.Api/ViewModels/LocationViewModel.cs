namespace DevChallenge.Api.ViewModels
{
    public class LocationViewModel
    {
        public string State { get; private set; }
        public string City { get; private set; }

        public LocationViewModel(string state, string city)
            => (State, City) = (state, city);
    }
}
