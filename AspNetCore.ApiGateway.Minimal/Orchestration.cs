using System.Text.Json.Serialization;

namespace AspNetCore.ApiGateway.Minimal
{
    public class Orchestration
    {
        [JsonPropertyOrder(1)]
        [JsonPropertyName("api")]
        public string Api { get; set; }

        [JsonIgnore()]
        public List<Route> ApiRoutes { get; set; }
    }

    public class ApiOrchestration : Orchestration
    {
        public ApiOrchestration()
        {
        }

        [JsonPropertyOrder(2)]
        [JsonPropertyName("routes")]
        public IEnumerable<Route> Routes { get; set; }
    }

    public class RouteBase
    {
        [JsonPropertyOrder(1)]
        [JsonPropertyName("key")]

        public string Key { get; set; }

        [JsonPropertyOrder(1)]
        [JsonPropertyName("apiKey")]

        public string ApiKey { get; set; }
    }

    public class Route : RouteBase
    {
        [JsonPropertyOrder(3)]
        [JsonPropertyName("verb")]
        public string Verb { get; set; }

        [JsonPropertyOrder(4)]
        [JsonPropertyName("downstreamPath")]
        public string DownstreamPath { get; set; }

        //[JsonPropertyOrder(5)]
        //[JsonPropertyName("requestJsonSchema")]
        //public JsonSchema RequestJsonSchema { get; set; }

        //[JsonPropertyOrder(6)]
        //[JsonPropertyName("responseJsonSchema")]
        //public JsonSchema ResponseJsonSchema { get; set; }
    }
}
