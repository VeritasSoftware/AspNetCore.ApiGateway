using System.Text.Json.Serialization;

namespace Weather.API
{
    public class AddWeatherTypeRequest
    {
        [JsonPropertyName("weatherType")]
        public string WeatherType { get; set; }
    }
}
