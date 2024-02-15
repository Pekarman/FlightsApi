namespace FlightsAPI.Models.Requests
{
    public class FlightParametersRequest
    {
        public string? FlightCompanyName { get; set; } = null;

        public string? DepartureAirport { get; set; } = null;

        public string? ArrivalAirport { get; set; } = null;

        public double? MinPrice { get; set; }

        public double? MaxPrice { get; set; }

        public string? OrderBy { get; set; }
    }
}