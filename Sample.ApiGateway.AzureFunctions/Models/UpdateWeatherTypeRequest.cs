using System.Text.Json.Serialization;

namespace Sample.ApiGateway.AzureFunctions
{
    public class UpdateWeatherTypeRequest
    {
        [JsonPropertyName("weatherType")]
        public string WeatherType { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }
    }
}
