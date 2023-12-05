using System.Text.Json.Serialization;

namespace Weather.API
{
    public class WeatherTypeResponse
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
