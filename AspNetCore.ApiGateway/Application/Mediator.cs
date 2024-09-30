using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR.Client;
using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
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

    public class GatewayEventSourceRouteInfo : GatewayRouteInfoBase
    {
        public EventSourceRouteInfo EventSourceRoute { get; set; }
    }

    public class RouteInfo
    {
        private IEnumerable<string> _routeParams = new List<string>();
        private bool _isPathTypeDetermined = false;
        private bool _isParameterizedRoute = false;

        public string Path { get; set; }
        internal bool IsParameterizedRoute
        {
            get
            {
                if (!_isPathTypeDetermined)
                {
                    _isParameterizedRoute = Regex.Match(this.Path, @"\{.+?\}", RegexOptions.IgnoreCase | RegexOptions.Compiled).Success;
                    _isPathTypeDetermined=true;
                }
                return _isParameterizedRoute;
            }
        }
        public Type ResponseType { get; set; }
        public Type RequestType { get; set; }
        public Func<ApiInfo, HttpRequest, Task<object>> Exec { get; set; }
        public HttpClientConfig HttpClientConfig { get; set; }
        public int ResponseCachingDurationInSeconds { get; set; } = -1;
        private IEnumerable<string> Params
        {
            get
            {
                if (_routeParams.Any())
                {
                    return _routeParams;
                }

                var m = Regex.Match(this.Path, @"^.*?(\{(?<param>.+?)\}.*?)*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                if (m.Success)
                {
                    _routeParams = m.Groups["param"].Captures.Select(x => x.Value).ToList();                    
                }

                return _routeParams;
            }
        }
        internal string GetPath(HttpRequest request)
        {
            var p = request.Query["parameters"][0];

            var allParams = p.Split('&');

            var paramDictionary = allParams.Select(x => x.Split('='));

            var paramValues = paramDictionary.Join(this.Params, x => x[0], y => y, (x, y) => new { Key = x[0], Value = x[1] }).ToList();

            var path = new string(this.Path.ToCharArray());

            paramValues.ForEach(x =>
            {
                path = Regex.Replace(path, $"{{{x.Key}}}", x.Value);
            });

            return path;
        }
    }

    public enum HubBroadcastType
    {
        All,
        Group,
        Individual
    }

    public enum EventSourcingType
    {
        EventStore
    }

    public class HubRouteInfo
    {
        public string InvokeMethod { get; set; }
        public string ReceiveMethod { get; set; }
        public string ReceiveGroup { get; set; }
        public HubBroadcastType BroadcastType { get; set; } = HubBroadcastType.All;
        public Type[] ReceiveParameterTypes { get; set; }
    }

    public enum EventSourcingOperationType
    {
        PublishOnly,
        SubscribeOnly,
        PublishSubscribe
    }

    public class EventSourceRouteInfo
    {
        public EventSourcingType Type { get; set; } = EventSourcingType.EventStore;
        public EventSourcingOperationType OperationType { get; set; } = EventSourcingOperationType.PublishSubscribe;
        public string StreamName { get; set; }
        public string GroupName { get; set; }

        public string ReceiveMethod { get; set; }
    }

    public class EventSourceMediator : IEventSourceMediator
    {
        public IApiOrchestrator ApiOrchestrator
        {
            get => _apiOrchestrator;
            set => _apiOrchestrator = value;
        }

        private IApiOrchestrator _apiOrchestrator;
        Dictionary<string, GatewayEventSourceRouteInfo> paths = new Dictionary<string, GatewayEventSourceRouteInfo>();

        public Dictionary<string, GatewayEventSourceRouteInfo> Paths => paths;
        
        public IHubMediator AddHub(string apiKey, Func<HubConnectionBuilder, HubConnection> connectionBuilder, string receiveKey = null)
        {
            return _apiOrchestrator.AddHub(apiKey, connectionBuilder, receiveKey);
        }

        public IMediator AddApi(string apiKey, params string[] baseUrls)
        {
            return _apiOrchestrator.AddApi(apiKey, baseUrls);
        }

        public IEventSourceMediator AddEventSource(string apiKey, Func<object> connectionBuilder, string routeKey)
        {
            _apiOrchestrator.AddEventSource(apiKey, connectionBuilder, routeKey);

            return this;
        }

        public IEventSourceMediator AddRoute(string key, EventSourceRouteInfo routeInfo)
        {
            var gatewayRouteInfo = new GatewayEventSourceRouteInfo
            {
                EventSourceRoute = routeInfo
            };

            paths.Add(key.ToLower(), gatewayRouteInfo);

            return this;
        }

        public GatewayEventSourceRouteInfo GetRoute(string key)
        {
            return paths[key.ToLower()];
        }

        public IEnumerable<EventSourceRoute> Routes => paths.Select(x => new EventSourceRoute
        {
            Key = x.Key,
            GroupName = x.Value.EventSourceRoute.GroupName,
            OperationType = x.Value.EventSourceRoute.OperationType.ToString(),
            ReceiveMethod = x.Value.EventSourceRoute.ReceiveMethod,
            StreamName = x.Value.EventSourceRoute.StreamName,
            Type = x.Value.EventSourceRoute.Type.ToString(),
        });
    }

    public class HubMediator : IHubMediator
    {
        private IApiOrchestrator _apiOrchestrator;
        Dictionary<string, GatewayHubRouteInfo> paths = new Dictionary<string, GatewayHubRouteInfo>();

        public Dictionary<string, GatewayHubRouteInfo> Paths => paths;

        public IApiOrchestrator ApiOrchestrator
        {
            get => _apiOrchestrator;
            set => _apiOrchestrator = value;
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

        public IEventSourceMediator AddEventSource(string apiKey, Func<object> connectionBuilder, string routeKey)
        {
            return _apiOrchestrator.AddEventSource(apiKey, connectionBuilder, routeKey);
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

        public IEnumerable<HubRoute> Routes => paths.Select(x => new HubRoute
        {
            Key = x.Key,
            BroadcastType = x.Value.HubRoute.BroadcastType.ToString(),
            InvokeMethod = x.Value.HubRoute.InvokeMethod,
            ReceiveGroup = x.Value.HubRoute.ReceiveGroup,
            ReceiveMethod = x.Value.HubRoute.ReceiveMethod,
            ReceiveParameterTypes = x.Value.HubRoute.ReceiveParameterTypes.Select(y => y.Name)
        });
    }

    public class Mediator : IMediator
    {
        IApiOrchestrator _apiOrchestrator;
        Dictionary<string, GatewayRouteInfo> paths = new Dictionary<string, GatewayRouteInfo>();

        public IApiOrchestrator ApiOrchestrator
        {
            get => _apiOrchestrator;
            set => _apiOrchestrator = value;
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
            DownstreamPath = x.Value?.Route?.Path?.ToString(),
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

        public IEventSourceMediator AddEventSource(string apiKey, Func<object> connectionBuilder, string routeKey)
        {
            return _apiOrchestrator.AddEventSource(apiKey, connectionBuilder, routeKey);
        }

        #endregion
    }

}
