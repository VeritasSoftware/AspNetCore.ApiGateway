using Newtonsoft.Json;

namespace ApiGateway.API
{
    public class AddWeatherTypeRequest
    {
        [JsonProperty("weatherType")]
        public string WeatherType { get; set; }
    }
}
