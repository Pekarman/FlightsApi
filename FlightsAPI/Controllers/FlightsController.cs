using FlightsAPI.Interfaces;
using FlightsAPI.Models;
using FlightsAPI.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FlightsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly ILogger<FlightsController> logger;

        private IMemoryCache cache;

        private ITestAviaProvider[] providers;

        private CancellationTokenSource cts;

        public FlightsController
            (
            ILogger<FlightsController> logger,
            IMemoryCache cache,
            ITestAviaProvider1 aviaProvider1,
            ITestAviaProvider2 aviaProvider2
            )
        {
            this.logger = logger;
            this.cache = cache;

            this.providers = [ aviaProvider1, aviaProvider2 ];

            this.cts = new CancellationTokenSource();
        }

        /// <summary>
        /// Gets all flights from all sources.
        /// </summary>
        /// <returns>List of flights.</returns>
        /// <response code="200">Returns the list of flights.</response>
        /// <response code="500">If there is some server error.</response>
        [HttpGet("getAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllFlights()
        {
            this.cts.CancelAfter(TimeSpan.FromSeconds(5));

            this.logger.Log(
                LogLevel.Information,
                $"{DateTime.UtcNow.ToLongTimeString()} : Trying to get flights."
                );

            List<Flight> flights;

            cache.TryGetValue("AllFlights", out flights);

            if (flights == null)
            {
                flights = new List<Flight>();

                foreach ( var provider in providers )
                {
                    try
                    {
                        if (this.cts.Token.IsCancellationRequested)
                        {
                            this.logger.Log(
                            LogLevel.Error,
                            $"{DateTime.UtcNow.ToLongTimeString()} : Data getting from Source={provider.GetProviderName()} cancelled due timeout."
                            );
                            break;
                        }

                        var localServiceToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
                        var result = await provider.GetAllFlightsAsync(localServiceToken);
                        flights.AddRange(result);
                    }
                    catch (Exception e)
                    {
                        this.logger.Log(
                            LogLevel.Error,
                            $"{DateTime.UtcNow.ToLongTimeString()} : Can't get flights from Source={provider.GetProviderName()}. Error: {e.Message}"
                            );
                    }
                }

                if (flights.Count == 0)
                {
                    this.logger.Log(
                        LogLevel.Error,
                        $"{DateTime.UtcNow.ToLongTimeString()} : Flights not found."
                        );

                    return NotFound("Flights not found.");
                }

                cache.Set("AllFlights", flights, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));

            }

            this.logger.Log(
                LogLevel.Information,
                $"{DateTime.UtcNow.ToLongTimeString()} : Flights got successful."
                );

            return Ok(flights);
        }

        /// <summary>
        /// Gets flights by filters and orders result (if required).
        /// </summary>
        /// <param name="parameters">Filters and orders.</param>
        /// <returns>List of filtered and ordered flights.</returns>
        /// <response code="200">Returns the list of filtered and sorted flights.</response>
        /// <response code="404">If flights not found</response>
        /// <response code="500">If there is some server error.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFlights([FromQuery] FlightParametersRequest parameters)
        {
            this.cts.CancelAfter(TimeSpan.FromSeconds(5));

            this.logger.Log(
                LogLevel.Information,
                $"{DateTime.UtcNow.ToLongTimeString()} : Trying to get flights."
                );

            var flights = new List<Flight>();

            foreach (var provider in providers)
            {
                try
                {
                    if (this.cts.Token.IsCancellationRequested)
                    {
                        this.logger.Log(
                        LogLevel.Error,
                        $"{DateTime.UtcNow.ToLongTimeString()} : Data getting from Source={provider.GetProviderName()} cancelled due timeout."
                        );
                        break;
                    }

                    var localServiceToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
                    var result = await provider.GetAllFlightsAsync(localServiceToken);
                    flights.AddRange(result);
                }
                catch (Exception e)
                {
                    this.logger.Log(
                        LogLevel.Error,
                        $"{DateTime.UtcNow.ToLongTimeString()} : Can't get flights from Source={provider.GetProviderName()}. Error: {e.Message}"
                        );
                }
            }

            flights = flights
                .Where(f => parameters.FlightCompanyName != null ? f.FlightCompanyName == parameters.FlightCompanyName : true)
                .Where(f => parameters.DepartureAirport != null ? f.DepartureAirport == parameters.DepartureAirport : true)
                .Where(f => parameters.ArrivalAirport != null ? f.ArrivalAirport == parameters.ArrivalAirport : true)
                .Where(f => parameters.MinPrice != null ? f.Price >= parameters.MinPrice : true)
                .Where(f => parameters.MaxPrice != null ? f.Price <= parameters.MaxPrice : true)
                .ToList();

            flights = flights.OrderBy(f =>
            {
                switch (parameters.OrderBy?.ToLower())
                {
                    case "price": return f.Price;
                    case "transfers": return f.Transfers;
                    default: return 0;
                }
            }).ToList();

            if (flights.Count == 0)
            {
                this.logger.Log(
                    LogLevel.Error,
                    $"{DateTime.UtcNow.ToLongTimeString()} : Flights not found."
                    );

                return NotFound("Flights not found.");
            }

            this.logger.Log(
                LogLevel.Information,
                $"{DateTime.UtcNow.ToLongTimeString()} : Flights got successful."
                );

            return Ok(flights);
        }

        /// <summary>
        /// Books a flight.
        /// </summary>
        /// <param name="request">Consists of Source and FlightId</param>
        /// <returns>Booked Flight.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /BookFlight
        ///     {
        ///        "source": 1,
        ///        "flightId": "ABC123"
        ///     }
        ///
        /// </remarks> 
        /// <response code="200">Returns the booked flight</response>
        /// <response code="400">If the request is null</response>
        /// <response code="500">If there is some server error.</response>
        [HttpPost("bookFlight")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> BookFlight([FromBody] BookFlightRequest request)
        {
            if (request == null)
            {
                this.logger.Log(
                    LogLevel.Error,
                    $"{DateTime.UtcNow.ToLongTimeString()} : Request is null. Operation aborted."
                    );

                return BadRequest("Request is null. Operation aborted.");
            }

            this.logger.Log(
                LogLevel.Information,
                $"{DateTime.UtcNow.ToLongTimeString()} : Trying to book flight with FlightId={request.FlightId} and Source={request.Source}."
                );

            Flight flight = new Flight();

            try
            {
                var provider = providers.Where(p => p.GetProviderName() == request.Source).FirstOrDefault();
                if (provider != null)
                {
                    flight = await provider.BookFlightAsync(request.FlightId);
                }
                else throw new Exception($"No provider with Source={request.Source}");
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
