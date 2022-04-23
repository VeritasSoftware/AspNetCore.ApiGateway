using Newtonsoft.Json;
using NJsonSchema;
using System.Collections.Generic;

namespace AspNetCore.ApiGateway
{
    public class Orchestration
    {
        public string Api { get; set; }

        public IEnumerable<RouteBase> Routes { get; set; }
    }

    public abstract class RouteBase
    {
        [JsonProperty(Order = 1)]
        public string Key { get; set; }        
    }

    public class Route : RouteBase
    {
        [JsonProperty(Order = 2)]
        public string Verb { get; set; }

        [JsonProperty(Order = 3)]
        public string Path { get; set; }

        [JsonProperty(Order = 4)]
        public JsonSchema RequestJsonSchema { get; set; }

        [JsonProperty(Order = 5)]
        public JsonSchema ResponseJsonSchema { get; set; }
    }

    public class HubRoute : RouteBase
    {
        [JsonProperty(Order = 2)]
        public string InvokeMethod { get; set; }

        [JsonProperty(Order = 3)]
        public string ReceiveMethod { get; set; }        

        [JsonProperty(Order = 4)]
        public string ReceiveGroup { get; set; }

        [JsonProperty(Order = 5)]
        public string BroadcastType { get; set; }

        [JsonProperty(Order = 6)]
        public IEnumerable<string> ReceiveParameterTypes { get; set; }
    }

    public class EventSourceRoute : RouteBase
    {
        [JsonProperty(Order = 2)]
        public string Type { get; set; }

        [JsonProperty(Order = 3)]
        public string ReceiveMethod { get; set; }

        [JsonProperty(Order = 4)]
        public string OperationType { get; set; }

        [JsonProperty(Order = 5)]
        public string StreamName { get; set; }

        [JsonProperty(Order = 6)]
        public string GroupName { get; set; }
    }

}
