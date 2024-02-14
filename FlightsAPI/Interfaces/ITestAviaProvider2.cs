﻿using FlightsAPI.Models;

namespace FlightsAPI.Interfaces
{
    public interface ITestAviaProvider2
    {
        public Task<IEnumerable<Flight>> GetFlightsAsync();
    }
}