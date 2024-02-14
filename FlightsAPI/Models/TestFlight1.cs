namespace FlightsAPI.Models
{
    public class TestFlight1
    {
        public string? GuId { get; set; }

        public string? AviaCompanyName { get; set; }

        public string? DepartureAirportName { get; set; }

        public DateTime? DepartureTime { get; set; }

        public string? DestinationAirportName { get; set; }

        public DateTime? DestinationTime { get; set; }

        public int NumberOfTransfers { get; set; }

        public double TotalAmount { get; set; }
    }
}
