using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using FailingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FailingApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SourceWeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<SourceWeatherForecastController> _logger;

        public SourceWeatherForecastController(ILogger<SourceWeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var randomError = new Random();

            //Here lets give a 50% chance of HttpRequestException
            if (randomError.Next(0, 2) == 0) 
            {
                throw new HttpRequestException("Mock HTTP Request Error");
            }
            else
            {
                var rng = new Random();
                return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();

            }
        }
    }
}
