using Newtonsoft.Json;

namespace AspNetCore.ApiGateway.Tests
{
    public class UpdateWeatherTypeRequest
    {
        [JsonProperty("weatherType")]
        public string WeatherType { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }
    }
}
