using System.Text.Json.Serialization;

namespace Sample.ApiGateway.AzureFunctions
{
    public class AddWeatherTypeRequest
    {
        [JsonPropertyName("weatherType")]
        public string WeatherType { get; set; }
    }
}
