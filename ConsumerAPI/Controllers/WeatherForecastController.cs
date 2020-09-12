using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ConsumerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;

namespace ConsumerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHttpClientFactory _client;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpClientFactory client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = new HttpResponseMessage();

            var maxRetry = 10;
            var requestAttempt = 0;

            var retryPolicy = Policy.HandleResult<HttpResponseMessage>(r => r.IsSuccessStatusCode == false)
                .WaitAndRetryAsync(
                    retryCount: maxRetry,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (response, sleep) =>
                    {
                        _logger.LogInformation($"Retry at {requestAttempt} of {maxRetry} GET Request {sleep}");
                    });

            response = await retryPolicy.ExecuteAsync(async () =>
            {
                requestAttempt++;
                var client = _client.CreateClient("FailingApi");
                return await client.GetAsync("sourceweatherforecast");
            });

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Success at {requestAttempt} of {maxRetry} GET Request");
                var weatherForecast = JsonConvert.DeserializeObject<IEnumerable<WeatherForecast>>(await response.Content.ReadAsStringAsync());
                return Ok(new { Message = "Success", Data = weatherForecast.ToList() });
            }
            else
            {
                return Ok(new { Message = "Failed", Data = "" });
            }

        }
    }
}
