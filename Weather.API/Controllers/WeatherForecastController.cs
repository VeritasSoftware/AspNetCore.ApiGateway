using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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

        static Random rng = new Random();

        private readonly WeatherForecast now = new WeatherForecast
        {
            Date = DateTime.Now,
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        };

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

        [HttpPut]
        [Route("types/update")]
        public IActionResult UpdateWeatherType([FromBody]UpdateWeatherTypeRequest weatherType)
        {
            Summaries[weatherType.Index] = weatherType.WeatherType;

            return Ok();
        }

        [HttpDelete]
        [Route("types/remove/{index}")]
        public IActionResult DeleteWeatherType(int index)
        {
            var list = Summaries.ToList();

            list.RemoveAt(index);

            Summaries = list.ToArray();

            return Ok();
        }

        [HttpPatch]
        [Route("forecast/patch")]
        public IActionResult Patch([FromBody] JsonPatchDocument<WeatherForecast> patch)
        {
            if (patch != null)
            {                
                patch.ApplyTo(now);

                return Ok(now);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

    }
}
