using System.Text.Json.Serialization;

namespace ApiGateway.API
{
    public class AddWeatherTypeRequest
    {
        [JsonPropertyName("weatherType")]
        public string WeatherType { get; set; }
    }
}
