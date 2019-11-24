using Newtonsoft.Json;

namespace Weather.API
{
    public class AddWeatherTypeRequest
    {
        [JsonProperty("weatherType")]
        public string WeatherType { get; set; }
    }
}
