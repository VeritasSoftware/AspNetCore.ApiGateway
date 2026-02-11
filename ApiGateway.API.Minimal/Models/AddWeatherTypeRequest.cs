using System.Text.Json.Serialization;

namespace ApiGateway.API.Minimal
{
    public class AddWeatherTypeRequest
    {
        [JsonPropertyName("weatherType")]
        public string WeatherType { get; set; }
    }
}
