using System.Text.Json.Serialization;

namespace ApiGateway.API
{
    public class WeatherTypeResponse
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
