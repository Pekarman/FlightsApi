using FlightsAPI.Models;

namespace FlightsAPI.Interfaces
{
    public interface ITestAviaProvider1
    {
        public Task<IEnumerable<Flight>> GetAllFlightsAsync();
    }
}
