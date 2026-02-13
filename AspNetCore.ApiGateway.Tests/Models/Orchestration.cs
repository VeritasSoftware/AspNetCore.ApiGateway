using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AspNetCore.ApiGateway.Tests
{
    public class Orchestration
    {
        [JsonPropertyName("api")]
        public string Api { get; set; }

        [JsonPropertyName("apiRoutes")]
        public IEnumerable<Route> ApiRoutes { get; set; }
    }

    public class Route
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("verb")]
        public string Verb { get; set; }

        [JsonPropertyName("downstreamPath")]
        public string DownstreamPath { get; set; }
    }
}
