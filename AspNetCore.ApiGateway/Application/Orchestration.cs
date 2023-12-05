using NJsonSchema;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AspNetCore.ApiGateway
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
        Api,
        Hub,
        EventSource
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

    public class HubOrchestration : Orchestration
    {
        public HubOrchestration()
        {
            OrchestrationType = OrchestationType.Hub;
        }

        [JsonPropertyOrder(2)]
        public IEnumerable<HubRoute> HubRoutes { get; set; }
    }

    public class EventSourceOrchestration : Orchestration
    {
        public EventSourceOrchestration()
        {
            OrchestrationType = OrchestationType.EventSource;
        }

        [JsonPropertyOrder(2)]
        public IEnumerable<EventSourceRoute> EventSourceRoutes { get; set; }
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

    public class HubRoute : RouteBase
    {
        [JsonPropertyOrder(3)]
        public string InvokeMethod { get; set; }

        [JsonPropertyOrder(4)]
        public string ReceiveMethod { get; set; }

        [JsonPropertyOrder(5)]
        public string ReceiveGroup { get; set; }

        [JsonPropertyOrder(6)]
        public string BroadcastType { get; set; }

        [JsonPropertyOrder(7)]
        public IEnumerable<string> ReceiveParameterTypes { get; set; }
    }

    public class EventSourceRoute : RouteBase
    {
        [JsonPropertyOrder(3)]
        public string Type { get; set; }

        [JsonPropertyOrder(4)]
        public string ReceiveMethod { get; set; }

        [JsonPropertyOrder(5)]
        public string OperationType { get; set; }

        [JsonPropertyOrder(6)]
        public string StreamName { get; set; }

        [JsonPropertyOrder(7)]
        public string GroupName { get; set; }
    }

}
