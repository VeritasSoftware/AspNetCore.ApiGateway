using System.Text.RegularExpressions;

namespace AspNetCore.ApiGateway.Minimal
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
        public string ApiKey { get; set; }
    }

    public class GatewayRouteInfo : GatewayRouteInfoBase
    {
        public GatewayVerb Verb { get; set; }

        public RouteInfo Route { get; set; }
    }  

    public class RouteInfo
    {
        private IEnumerable<string> _routeParams = new List<string>();
        private bool _isPathTypeDetermined = false;
        private bool _isParameterizedRoute = false;
        private string _path = string.Empty;

        public string Path
        {
            get => _path;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(Path));
                }
                _path = value.Trim();
            }
        }
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
            if (string.IsNullOrWhiteSpace(routeKey))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(routeKey));
            }

            var gatewayRouteInfo = new GatewayRouteInfo
            {
                Verb = verb,
                ApiKey = MediatorHelper.CurrentApiKey,
                Route = routeInfo
            };

            paths.Add(routeKey.Trim().ToLower(), gatewayRouteInfo);

            return this;
        }

        public IMediator AddRoute(string routeKey, GatewayVerb verb, Func<ApiInfo, HttpRequest, Task<object>> exec)
        {
            if (string.IsNullOrWhiteSpace(routeKey))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(routeKey));
            }

            if (exec == null)
            {
                throw new ArgumentNullException(nameof(exec));
            }

            var gatewayRouteInfo = new GatewayRouteInfo
            {
                Verb = verb,
                ApiKey = MediatorHelper.CurrentApiKey,
                Route = new RouteInfo
                {
                    Exec = exec
                }
            };

            paths.Add(routeKey.Trim().ToLower(), gatewayRouteInfo);

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
            RouteKey = x.Key,
            ApiKey = x.Value?.ApiKey,
            Verb = x.Value?.Verb.ToString(),
            DownstreamPath = x.Value?.Route?.Path?.ToString(),
        });
    }

}
