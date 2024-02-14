namespace FlightsAPI.Models
{
    public class TestFlight2
    {
        public string? Id { get; set; }

        public string? CompanyName { get; set; }

        public string? DepAirport {  get; set; }

        public string? DepTime { get; set; }

        public string? DestAirport { get; set; }

        public string? DestTime { get; set; }

        public int TransfersQuantity { get; set; }

        public decimal FlightCost { get; set; }
    }
}
