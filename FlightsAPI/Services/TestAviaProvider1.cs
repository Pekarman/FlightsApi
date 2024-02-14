using AutoMapper;
using FlightsAPI.Interfaces;
using FlightsAPI.Models;

namespace FlightsAPI.Services
{
    public class TestAviaProvider1 : ITestAviaProvider1
    {
        private readonly IMapper mapper;

        public TestAviaProvider1(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public async Task<IEnumerable<Flight>> GetAllFlightsAsync()
        {
            var flightsSrc = new List<TestFlight1>();

            var flights = this.mapper.Map<List<Flight>>(flightsSrc);

            await Task.Delay(1000);

            return flights;
        }
    }
}
