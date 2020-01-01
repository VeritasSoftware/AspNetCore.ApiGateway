using Newtonsoft.Json;

namespace AspNetCore.ApiGateway.Tests
{
    public class AddWeatherTypeRequest
    {
        [JsonProperty("weatherType")]
        public string WeatherType { get; set; }
    }
}
