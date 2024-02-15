using AutoMapper;
using FlightsAPI.Interfaces;
using FlightsAPI.Models;

namespace FlightsAPI.Services
{
    public class TestAviaProvider2 : ITestAviaProvider2
    {
        TestFlight2[] testFlights =
        {
            new TestFlight2
            {
                Id = "FL001",
                CompanyName = "Airline A",
                DepAirport = "JFK",
                DepTime = DateTime.UtcNow.AddHours(3),
                DestAirport = "LHR",
                DestTime = DateTime.UtcNow.AddHours(10),
                TransfersQuantity = 1,
                FlightCost = 250.50m
            },
            new TestFlight2
            {
                Id = "FL002",
                CompanyName = "Airline B",
                DepAirport = "ORD",
                DepTime = DateTime.UtcNow.AddHours(1),
                DestAirport = "CDG",
                DestTime = DateTime.UtcNow.AddHours(6),
                TransfersQuantity = 2,
                FlightCost = 350.00m
            },
            new TestFlight2
            {
                Id = "FL003",
                CompanyName = "Airline C",
                DepAirport = "LAX",
                DepTime = DateTime.UtcNow.AddHours(2),
                DestAirport = "HKG",
                DestTime = DateTime.UtcNow.AddHours(4),
                TransfersQuantity = 0,
                FlightCost = 180.00m
            },
            new TestFlight2
            {
                Id = "FL004",
                CompanyName = "Airline D",
                DepAirport = "DFW",
                DepTime = DateTime.UtcNow.AddHours(3),
                DestAirport = "SYD",
                DestTime = DateTime.UtcNow.AddHours(8),
                TransfersQuantity = 1,
                FlightCost = 220.25m
            },
            new TestFlight2
            {
                Id = "FL005",
                CompanyName = "Airline E",
                DepAirport = "ATL",
                DepTime = DateTime.UtcNow.AddHours(1),
                DestAirport = "NRT",
                DestTime = DateTime.UtcNow.AddHours(9),
                TransfersQuantity = 3,
                FlightCost = 440.80m
            }
        };

        private readonly IMapper mapper;

        public TestAviaProvider2(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public async Task<IEnumerable<Flight>> GetFlightsAsync()
        {
            var flights = this.mapper.Map<List<Flight>>(this.testFlights.ToList());

            await Task.Delay(200);

            return flights;
        }

        public async Task<Flight> BookFlightAsync(string flightId)
        {
            var flights = this.mapper.Map<List<Flight>>(this.testFlights.ToList());

            var flight = flights.Where(f => f.FlightId == flightId).FirstOrDefault();

            if (flight == null) throw new Exception($"Flight with flightId={flightId} not found.");

            await Task.Delay(200);

            return flight;
        }
    }
}
