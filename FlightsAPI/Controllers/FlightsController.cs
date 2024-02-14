using FlightsAPI.Interfaces;
using FlightsAPI.Models;
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

        [HttpGet]
        public async Task<IEnumerable<Flight>> Get()
        {
            var flights = new List<Flight>();

            try
            {
                this.logger.Log(LogLevel.Information, $"{DateTime.UtcNow.ToLongTimeString()} : Trying to get flights.");

                var flights1 = await this.aviaProvider1.GetAllFlightsAsync();
                flights.AddRange(flights1);

                var flights2 = await this.aviaProvider2.GetFlightsAsync();
                flights.AddRange(flights2);
            }
            catch (Exception e)
            {
                this.logger.Log(LogLevel.Error, $"{DateTime.UtcNow.ToLongTimeString()} : Can't get flights. Error: {e.Message}");
            }

            return flights;
        }

        [HttpPost("bookFlight")]
        public void Post([FromBody] string flightId)
        {
        }
    }
}
