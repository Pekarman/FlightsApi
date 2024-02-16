using FlightsAPI.Models;
using FlightsAPI.Models.Requests;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace FlightsApi.Tests
{
    public class FlightsControllerTests
    {
        private readonly HttpClient client;

        public FlightsControllerTests()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7194/");
        }

        [Fact]
        public async void GetAllFlights()
        {
            var response = await client.GetAsync("/api/Flights/getAll");
            Assert.True(response.IsSuccessStatusCode);

            var result = response.Content.ReadAsStringAsync().Result;
            Assert.NotNull(result);

            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(result);
            Assert.NotNull(flights);
            Assert.True(flights.Count > 0);
        }

        [Theory]
        [InlineData(300)]
        [InlineData(200)]
        public async void GetFlightsByMinPriceFilter(double minPrice)
        {
            var response = await client.GetAsync($"/api/Flights?MinPrice={minPrice}");
            Assert.True(response.IsSuccessStatusCode);

            var result = response.Content.ReadAsStringAsync().Result;
            Assert.NotNull(result);

            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(result);
            Assert.NotNull(flights);
            Assert.True(flights.Count > 0);

            flights.ForEach(f => Assert.True(f.Price >= minPrice));
        }

        [Theory]
        [InlineData(300)]
        [InlineData(200)]
        public async void GetFlightsByMaxPriceFilter(double maxPrice)
        {
            var response = await client.GetAsync($"/api/Flights?MaxPrice={maxPrice}");
            Assert.True(response.IsSuccessStatusCode);

            var result = response.Content.ReadAsStringAsync().Result;
            Assert.NotNull(result);

            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(result);
            Assert.NotNull(flights);
            Assert.True(flights.Count > 0);

            flights.ForEach(f => Assert.True(f.Price <= maxPrice));
        }

        [Fact]
        public async void BookFlight()
        {
            var response = await client.GetAsync("/api/Flights/getAll");
            Assert.True(response.IsSuccessStatusCode);

            var result = response.Content.ReadAsStringAsync().Result;
            Assert.NotNull(result);

            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(result);
            Assert.NotNull(flights);

            var request = new BookFlightRequest()
            {
                Source = flights[0].Source,
                FlightId = flights[0].FlightId
            };

            var myContent = JsonConvert.SerializeObject(request);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var bookFlightResponse = await client.PostAsync("/api/Flights/bookFlight", byteContent);
            Assert.True(bookFlightResponse.IsSuccessStatusCode);

            var bookFlightResult = bookFlightResponse.Content.ReadAsStringAsync().Result;
            Assert.NotNull(bookFlightResult);

            var flight = JsonConvert.DeserializeObject<Flight>(bookFlightResult);
            Assert.NotNull(flight);

            Assert.True(flight.Source == request.Source && flight.FlightId == request.FlightId);
        }
    }
}