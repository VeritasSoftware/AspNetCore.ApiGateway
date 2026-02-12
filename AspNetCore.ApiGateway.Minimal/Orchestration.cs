using NJsonSchema;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AspNetCore.ApiGateway.Minimal
{
    public class Orchestration
    {
        [JsonPropertyOrder(1)]
        public string Api { get; set; }

        [JsonPropertyOrder(2)]
        public OrchestationType OrchestrationType { get; set; }

        [JsonIgnore()]
        public List<RouteBase> Routes { get; set; }
    }

    public enum OrchestationType
    {
        Api
    }

    public class ApiOrchestration : Orchestration
    {
        public ApiOrchestration()
        {
            OrchestrationType = OrchestationType.Api;
        }

        [JsonPropertyOrder(2)]
        public IEnumerable<Route> ApiRoutes { get; set; }        
    }

    public class RouteBase
    {
        [JsonPropertyOrder(1)]
        public string Key { get; set; }
    }

    public class Route : RouteBase
    {
        [JsonPropertyOrder(3)]
        public string Verb { get; set; }

        [JsonPropertyOrder(4)]
        public string DownstreamPath { get; set; }

        [JsonPropertyOrder(5)]
        public JsonSchema RequestJsonSchema { get; set; }

        [JsonPropertyOrder(6)]
        public JsonSchema ResponseJsonSchema { get; set; }
    }
}
