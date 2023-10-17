using DevChallenge.Api.ViewModels;

namespace DevChallenge.Models
{
    public class Location
    {
        public string Id { get; private set; }
        public string State { get; private set; }
        public string City { get; private set; }

        public Location(string id, string state, string city)
            => (Id, State, City) = (id, state, city);

        public void Update(LocationViewModel viewModel)
            => (State, City) = (viewModel.State, viewModel.City);
    }
}
