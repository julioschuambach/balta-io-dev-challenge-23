namespace DevChallenge.Models
{
    public class Ibge
    {
        public string Id { get; private set; }
        public string State { get; private set; }
        public string City { get; private set; }

        public Ibge(string id, string state, string city)
            => (Id, State, City) = (id, state, city);
    }
}
