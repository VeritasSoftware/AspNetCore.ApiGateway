using Microsoft.AspNetCore.Http;
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

        public Action<HttpClient> CustomizeDefaultHttpClient { get; set; }
    }

    public class GatewayRouteInfo
    {
        public GatewayVerb Verb { get; set; }

        public RouteInfo Route { get; set; }
    }

    public class RouteInfo
    {
        public string Path { get; set; }
        public Type ResponseType { get; set; }
        public Type RequestType { get; set; }
        public Func<ApiInfo, HttpRequest, Task<object>> Exec { get; set; }
        public HttpClientConfig HttpClientConfig { get; set; }
    }


    public class Mediator : IMediator
    {
        readonly IApiOrchestrator _apiOrchestrator;
        Dictionary<string, GatewayRouteInfo> paths = new Dictionary<string, GatewayRouteInfo>();
        public Mediator(IApiOrchestrator apiOrchestrator)
        {
            _apiOrchestrator = apiOrchestrator;
        }

        public IMediator AddRoute(string key, GatewayVerb verb, RouteInfo routeInfo)
        {
            var gatewayRouteInfo = new GatewayRouteInfo
            {
                Verb = verb,
                Route = routeInfo
            };

            paths.Add(key.ToLower(), gatewayRouteInfo);

            return this;
        }

        public IMediator AddRoute(string key, GatewayVerb verb, Func<ApiInfo, HttpRequest, Task<object>> exec)
        {
            var gatewayRouteInfo = new GatewayRouteInfo
            {
                Verb = verb,
                Route = new RouteInfo
                {
                    Exec = exec
                }
            };

            paths.Add(key.ToLower(), gatewayRouteInfo);

            return this;
        }

        public IMediator AddApi(string apiKey, string baseUrl)
        {
            _apiOrchestrator.AddApi(apiKey, baseUrl);

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
        #endregion
    }

}
