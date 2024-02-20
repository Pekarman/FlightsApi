namespace FlightsAPI.Models.Requests
{
    /// <summary>
    /// Request to book flight
    /// </summary>
    public class BookFlightRequest
    {
        public int? Source { get; set; }

        public string? FlightId { get; set; }
    }
}
