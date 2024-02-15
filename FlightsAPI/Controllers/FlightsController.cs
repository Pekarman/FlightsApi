using FlightsAPI.Interfaces;
using FlightsAPI.Models;
using FlightsAPI.Models.Enums;
using FlightsAPI.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FlightsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly ILogger<FlightsController> logger;

        private readonly ITestAviaProvider1 aviaProvider1;
        private readonly ITestAviaProvider2 aviaProvider2;

        public FlightsController
            (
            ILogger<FlightsController> logger,
            ITestAviaProvider1 aviaProvider1,
            ITestAviaProvider2 aviaProvider2
            )
        {
            this.logger = logger;
            this.aviaProvider1 = aviaProvider1;
            this.aviaProvider2 = aviaProvider2;
        }

        /// <summary>
        /// Gets flights by filters and orders result (if required).
        /// </summary>
        /// <param name="parameters">Filters and orders.</param>
        /// <returns>List of filtered and ordered flights.</returns>
        [HttpGet]
        public async Task<IActionResult> GetFlights([FromQuery] FlightParametersRequest parameters)
        {
            this.logger.Log(
                LogLevel.Information,
                $"{DateTime.UtcNow.ToLongTimeString()} : Trying to get flights."
                );

            var flights = new List<Flight>();

            try
            {
                var flights1 = await this.aviaProvider1.GetAllFlightsAsync();
                flights.AddRange(flights1);

                var flights2 = await this.aviaProvider2.GetFlightsAsync();
                flights.AddRange(flights2);
            }
            catch (Exception e)
            {
                this.logger.Log(
                    LogLevel.Error,
                    $"{DateTime.UtcNow.ToLongTimeString()} : Can't get flights. Error: {e.Message}"
                    );

                return Problem(e.Message);
            }

            flights = flights.Where(f => 
                parameters.FlightCompanyName != null
                ? f.FlightCompanyName == parameters.FlightCompanyName
                : parameters.DepartureAirport != null
                ? f.DepartureAirport == parameters.DepartureAirport
                : parameters.ArrivalAirport != null
                ? f.ArrivalAirport == parameters.ArrivalAirport
                : true
                ).ToList();

            flights = flights.OrderBy(f =>
            {
                switch (parameters.OrderBy)
                {
                    case "Price": return f.Price;
                    case "Transfers": return f.Transfers;
                    default: return 0;
                }
            }).ToList();

            this.logger.Log(
                LogLevel.Information,
                $"{DateTime.UtcNow.ToLongTimeString()} : Flights got successful."
                );

            return Ok(flights);
        }

        [HttpPost("bookFlight")]
        public async Task<IActionResult> BookFlight([FromBody] BookFlightRequest request)
        {
            if (request == null)
            {
                this.logger.Log(
                    LogLevel.Error,
                    $"{DateTime.UtcNow.ToLongTimeString()} : Request is null. Operation aborted."
                    );

                return Problem("Request is null. Operation aborted.");
            }

            this.logger.Log(
                LogLevel.Information,
                $"{DateTime.UtcNow.ToLongTimeString()} : Trying to book flight with FlightId={request.FlightId} and Source={request.Source}."
                );

            Flight flight = new Flight();

            try
            {
                switch (request.Source)
                {
                    case (int)SourcesEnum.TestAviaProvider1:
                        flight = await this.aviaProvider1.BookFlightAsync(request.FlightId);
                        break;
                    case (int)SourcesEnum.TestAviaProvider2:
                        flight = await this.aviaProvider2.BookFlightAsync(request.FlightId);
                        break;
                    default: break;
                }
            }
            catch (Exception e)
            {
                this.logger.Log(
                    LogLevel.Error,
                    $"{DateTime.UtcNow.ToLongTimeString()} : Can't book flight. Error: {e.Message}"
                    );

                return Problem(e.Message);
            }

            this.logger.Log(
                LogLevel.Information,
                $"{DateTime.UtcNow.ToLongTimeString()} : Flight booked successful."
                );

            return Ok(flight);
        }
    }
}
