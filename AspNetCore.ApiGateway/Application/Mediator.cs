using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR.Client;
using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway
{
    public enum GatewayVerb
    {
        GET,
        HEAD,
        POST,
        PUT,
        PATCH,
        DELETE
    }

    public class HttpClientConfig
    {
        public Func<HttpClient> HttpClient { get; set; }

        public Func<HttpContent> HttpContent { get; set; }

        public Action<HttpClient, HttpRequest> CustomizeDefaultHttpClient { get; set; }
    }

    public abstract class GatewayRouteInfoBase
    {

    }

    public class GatewayRouteInfo : GatewayRouteInfoBase
    {
        public GatewayVerb Verb { get; set; }

        public RouteInfo Route { get; set; }
    }

    public class GatewayHubRouteInfo : GatewayRouteInfoBase
    {
        public GatewayVerb Verb { get; set; } = GatewayVerb.POST;
        public HubRouteInfo HubRoute { get; set; }
    }

    public class RouteInfo
    {
        public string Path { get; set; }
        public Type ResponseType { get; set; }
        public Type RequestType { get; set; }
        public Func<ApiInfo, HttpRequest, Task<object>> Exec { get; set; }
        public HttpClientConfig HttpClientConfig { get; set; }
    }

    public enum HubBroadcastType
    {
        All,
        Group,
        Individual
    }

    public class HubRouteInfo
    {
        public string InvokeMethod { get; set; }
        public string ReceiveMethod { get; set; }
        public string ReceiveGroup { get; set; }
        public HubBroadcastType BroadcastType { get; set; } = HubBroadcastType.All;
        public Type[] ReceiveParameterTypes { get; set; }
    }


    public class HubMediator : IHubMediator
    {
        private readonly IApiOrchestrator _apiOrchestrator;
        Dictionary<string, GatewayHubRouteInfo> paths = new Dictionary<string, GatewayHubRouteInfo>();

        public Dictionary<string, GatewayHubRouteInfo> Paths => paths;

        public HubMediator(IApiOrchestrator apiOrchestrator)
        {
            _apiOrchestrator = apiOrchestrator;
        }

        public IHubMediator AddHub(string apiKey, Func<HubConnectionBuilder, HubConnection> connectionBuilder, string receiveKey = null)
        {
            _apiOrchestrator.AddHub(apiKey, connectionBuilder, receiveKey);

            return this;
        }

        public IMediator AddApi(string apiKey, params string[] baseUrls)
        {
            _apiOrchestrator.AddApi(apiKey, baseUrls);

            return _apiOrchestrator.GetApi(apiKey).Mediator;
        }

        public IHubMediator AddRoute(string key, HubRouteInfo routeInfo)
        {
            var gatewayRouteInfo = new GatewayHubRouteInfo
            {
                HubRoute = routeInfo
            };

            paths.Add(key.ToLower(), gatewayRouteInfo);

            return this;
        }

        public GatewayHubRouteInfo GetRoute(string key)
        {
            return paths[key.ToLower()];
        }

        public IEnumerable<Route> Routes => paths.Select(x => new Route
        {
            Key = x.Key,
        });
    }

    public class Mediator : IMediator
    {
        readonly IApiOrchestrator _apiOrchestrator;
        Dictionary<string, GatewayRouteInfo> paths = new Dictionary<string, GatewayRouteInfo>();
        public Mediator(IApiOrchestrator apiOrchestrator)
        {
            _apiOrchestrator = apiOrchestrator;
        }

        public IMediator AddRoute(string routeKey, GatewayVerb verb, RouteInfo routeInfo)
        {
            var gatewayRouteInfo = new GatewayRouteInfo
            {
                Verb = verb,
                Route = routeInfo
            };

            paths.Add(routeKey.ToLower(), gatewayRouteInfo);

            return this;
        }

        public IMediator AddRoute(string routeKey, GatewayVerb verb, Func<ApiInfo, HttpRequest, Task<object>> exec)
        {
            var gatewayRouteInfo = new GatewayRouteInfo
            {
                Verb = verb,
                Route = new RouteInfo
                {
                    Exec = exec
                }
            };

            paths.Add(routeKey.ToLower(), gatewayRouteInfo);

            return this;
        }

        public IMediator AddApi(string apiKey, params string[] baseUrls)
        {
            _apiOrchestrator.AddApi(apiKey, baseUrls);

            return _apiOrchestrator.GetApi(apiKey).Mediator;
        }

        public GatewayRouteInfo GetRoute(string key)
        {
            return paths[key.ToLower()];
        }

        public IEnumerable<Route> Routes => paths.Select(x => new Route 
        { 
            Key = x.Key, 
            Verb = x.Value?.Verb.ToString(),
            RequestJsonSchema = GetJsonSchema(x.Value?.Route?.RequestType),
            ResponseJsonSchema = GetJsonSchema(x.Value?.Route?.ResponseType)
        });

        #region Private Methods
        private JsonSchema GetJsonSchema(Type type)
        {
            if (type == null)
            {
                return null;
            }

            return JsonSchema.FromType(type);
        }

        public IHubMediator AddHub(string apiKey, Func<HubConnectionBuilder, HubConnection> connectionBuilder, string receiveKey = null)
        {
            return _apiOrchestrator.AddHub(apiKey, connectionBuilder, receiveKey);
        }

        #endregion
    }

}
