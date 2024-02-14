using AutoMapper;
using FlightsAPI.Interfaces;
using FlightsAPI.Models;

namespace FlightsAPI.Services
{
    public class TestAviaProvider2 : ITestAviaProvider2
    {
        private readonly IMapper mapper;

        public TestAviaProvider2(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public async Task<IEnumerable<Flight>> GetFlightsAsync()
        {
            var flightsSrc = new List<TestFlight2>();

            var flights = this.mapper.Map<List<Flight>>(flightsSrc);

            await Task.Delay(1000);

            return flights;
        }
    }
}
