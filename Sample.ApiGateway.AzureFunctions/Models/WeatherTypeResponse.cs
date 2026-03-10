using System.Text.Json.Serialization;

namespace Sample.ApiGateway.AzureFunctions
{
    public class WeatherTypeResponse
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
