using System.Text.Json.Serialization;

namespace AspNetCore.ApiGateway.Tests
{
    public class WeatherTypeResponse
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
