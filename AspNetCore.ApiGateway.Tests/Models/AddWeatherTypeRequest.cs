using System.Text.Json.Serialization;

namespace AspNetCore.ApiGateway.Tests
{
    public class AddWeatherTypeRequest
    {
        [JsonPropertyName("weatherType")]
        public string WeatherType { get; set; }
    }
}
