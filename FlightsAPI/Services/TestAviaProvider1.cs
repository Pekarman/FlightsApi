﻿using AutoMapper;
using FlightsAPI.Interfaces;
using FlightsAPI.Models;
using FlightsAPI.Models.Enums;

namespace FlightsAPI.Services
{
    public class TestAviaProvider1 : ITestAviaProvider1
    {
        TestFlight1[] testFlights = 
        {
            new TestFlight1
            {
                GuId = "ABC123",
                AviaCompanyName = "Airline A",
                DepartureAirportName = "Airport X",
                DepartureTime = DateTime.UtcNow,
                DestinationAirportName = "Airport Y",
                DestinationTime = DateTime.UtcNow.AddHours(3),
                NumberOfTransfers = 1,
                TotalAmount = 250
            },
            new TestFlight1
            {
                GuId = "GTF689",
                AviaCompanyName = "Airline D",
                DepartureAirportName = "Airport Y",
                DepartureTime = DateTime.UtcNow.AddHours(2),
                DestinationAirportName = "Airport Z",
                DestinationTime = DateTime.UtcNow.AddHours(6),
                NumberOfTransfers = 1,
                TotalAmount = 440
            },
            new TestFlight1
            {
                GuId = "XYZ789",
                AviaCompanyName = "Airline B",
                DepartureAirportName = "Airport X",
                DepartureTime = DateTime.UtcNow.AddHours(4),
                DestinationAirportName = "Airport Z",
                DestinationTime = DateTime.UtcNow.AddHours(7),
                NumberOfTransfers = 2,
                TotalAmount = 350
            },
            new TestFlight1
            {
                GuId = "JHT647",
                AviaCompanyName = "Airline F",
                DepartureAirportName = "Airport V",
                DepartureTime = DateTime.UtcNow.AddHours(3),
                DestinationAirportName = "Airport X",
                DestinationTime = DateTime.UtcNow.AddHours(8),
                NumberOfTransfers = 0,
                TotalAmount = 310
            },
            new TestFlight1
            {
                GuId = "JRY749",
                AviaCompanyName = "Airline A",
                DepartureAirportName = "Airport Z",
                DepartureTime = DateTime.UtcNow.AddHours(1),
                DestinationAirportName = "Airport Y",
                DestinationTime = DateTime.UtcNow.AddHours(4),
                NumberOfTransfers = 1,
                TotalAmount = 420
            }
        };

        private readonly IMapper mapper;

        public TestAviaProvider1(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public int GetProviderName() => (int)SourcesEnum.TestAviaProvider1;

        public async Task<IEnumerable<Flight>> GetAllFlightsAsync(CancellationToken token)
        {
            var flights = new List<Flight>();
            try
            {
                flights = this.mapper.Map<List<Flight>>(this.testFlights.ToList());

                await Task.Delay(2000, token);
            }
            catch (OperationCanceledException)
            {
                throw new Exception("Task cancelled due to timeout.");
            }

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
