using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Get()
        {
            var rng = new Random();
            return Ok(await Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray()));
        }

        [HttpGet]
        [Route("types")]
        public async Task<IActionResult> GetWeatherTypes()
        {
            return Ok(await Task.FromResult(Summaries));
        }

        [HttpGet]
        [Route("types/{index}")]
        public async Task<IActionResult> GetWeatherTypes(int index)
        {
            if (index < 0 || index >= Summaries.Length)
            {
                return BadRequest("Invalid index");
            }

            var result = new WeatherTypeResponse { Type = Summaries[index] };

            return Ok(await Task.FromResult(result));
        }

        [HttpPost]
        [Route("types/add")]
        public async Task<IActionResult> AddWeatherType([FromBody]AddWeatherTypeRequest weatherType)
        {
            var list = Summaries.ToList();
            
            list.Add(weatherType.WeatherType);

            Summaries = list.ToArray();

            return Ok(await Task.FromResult(Summaries));
        }

        [HttpPut]
        [Route("types/update")]
        public async Task<IActionResult> UpdateWeatherType([FromBody]UpdateWeatherTypeRequest weatherType)
        {
            Summaries[weatherType.Index] = weatherType.WeatherType;

            await Task.CompletedTask;

            return Ok();
        }

        [HttpDelete]
        [Route("types/remove/{index}")]
        public async Task<IActionResult> DeleteWeatherType(int index)
        {
            var list = Summaries.ToList();

            list.RemoveAt(index);

            Summaries = list.ToArray();

            await Task.CompletedTask;

            return Ok();
        }

        [HttpPatch]
        [Route("forecast/patch")]
        public async Task<IActionResult> Patch([FromBody] JsonPatchDocument<WeatherForecast> patch)
        {
            if (patch != null)
            {                
                patch.ApplyTo(now);

                await Task.CompletedTask;

                return Ok(now);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

    }
}
