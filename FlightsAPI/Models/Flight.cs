
namespace FlightsAPI.Models
{
    public class Flight
    {
        public int? Source { get; set; }

        public string? FlightId { get; set; }

        public string? FlightCompanyName { get; set; }

        public string? DepartureAirport {  get; set; }

        public DateTime? DepartureDateTime { get; set; } = default;

        public string? ArrivalAirport {  get; set; }
        
        public DateTime? ArrivalDateTime { get; set; } = default;

        public int Transfers {  get; set; }

        public double Price { get; set; }        
    }
}
