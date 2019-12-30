using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway
{
    public enum GatewayVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class HttpClientConfig
    {
        public Func<HttpClient> HttpClient { get; set; }

        public Func<HttpContent> HttpContent { get; set; }
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
            Request = this.GetJsonSchema(x.Value?.Route?.RequestType),
            Response = this.GetJsonSchema(x.Value?.Route?.ResponseType)
        });

        #region Private Methods
        private object GetObject(Type type)
        {
            if (type == null)
            {
                return null;
            }

            try
            {
                if (type.IsInterface && type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type))
                {
                    var instListType = Activator.CreateInstance(type.GetGenericArguments()[0]);

                    var props = instListType.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

                    PadStringProperties(instListType, props);

                    var list = new ArrayList();
                    list.Add(instListType);

                    return list;
                }

                if (type.IsArray)
                {
                    type = typeof(ArrayList);
                }

                var instance = Activator.CreateInstance(type);

                var properties = instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (var p in properties.Where(x => x.PropertyType.IsClass))
                {
                    p.SetValue(instance, GetObject(p.PropertyType));
                }

                foreach (var p in properties.Where(x => x.PropertyType.IsInterface && x.PropertyType.IsGenericType
                                                                && typeof(IEnumerable).IsAssignableFrom(x.PropertyType)))
                {
                    p.SetValue(instance, GetObject(p.PropertyType));
                }

                PadStringProperties(instance, properties);

                return instance;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string GetJsonSchema(Type type)
        {
            if (type == null)
            {
                return null;
            }

            try
            {
                var obj = GetObject(type);

                return JsonConvert.SerializeObject(obj);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void PadStringProperties(object instance, PropertyInfo[] properties)
        {
            foreach (var p in properties.Where(x => x.PropertyType == typeof(string)))
            {
                p.SetValue(instance, string.Empty);
            }
        }
        #endregion
    }

}
