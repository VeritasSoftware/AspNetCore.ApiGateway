using System.Text.Json.Serialization;

namespace ApiGateway.API.Minimal
{
    public class WeatherTypeResponse
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
