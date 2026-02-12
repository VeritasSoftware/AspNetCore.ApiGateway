using System.Text.Json.Serialization;

namespace ApiGateway.API.Minimal
{
    public class UpdateWeatherTypeRequest
    {
        [JsonPropertyName("weatherType")]
        public string WeatherType { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }
    }
}
