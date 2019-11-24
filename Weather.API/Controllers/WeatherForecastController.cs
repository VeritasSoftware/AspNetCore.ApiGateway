using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Weather.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("forecast")]
        public IEnumerable<WeatherForecast> Get()
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

        [HttpGet]
        [Route("types")]
        public string[] GetWeatherTypes()
        {
            return Summaries;
        }

        [HttpGet]
        [Route("types/{index}")]
        public WeatherTypeResponse GetWeatherTypes(int index)
        {
            return new WeatherTypeResponse { Type = Summaries[index] };
        }

        [HttpPost]
        [Route("types/add")]
        public string[] AddWeatherType([FromBody]AddWeatherTypeRequest weatherType)
        {
            var list = Summaries.ToList();
            
            list.Add(weatherType.WeatherType);

            Summaries = list.ToArray();

            return Summaries;
        }

        [HttpDelete]
        [Route("types/remove/{index}")]
        public string[] DeleteWeatherTypes(int index)
        {
            var list = Summaries.ToList();

            list.RemoveAt(index);

            Summaries = list.ToArray();

            return Summaries;
        }
    }
}
