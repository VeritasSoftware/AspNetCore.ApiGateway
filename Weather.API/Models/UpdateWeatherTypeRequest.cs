using Newtonsoft.Json;

namespace Weather.API
{
    public class UpdateWeatherTypeRequest
    {
        [JsonProperty("weatherType")]
        public string WeatherType { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }
    }
}
