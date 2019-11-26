using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway
{
    public class HttpClientConfig
    {
        public Func<HttpClient> HttpClient { get; set; }

        public Func<HttpContent> HttpContent { get; set; }
    }

    public class RouteInfo
    {
        public string Path { get; set; }
        public Type ResponseType { get; set; }
        public Type RequestType { get; set; }
        public Func<ApiInfo, RouteInfo, HttpRequest, Task<object>> Exec { get; set; }
        public HttpClientConfig HttpClientConfig { get; set; }
    }


    public class Mediator : IMediator
    {
        readonly IApiOrchestrator _apiOrchestrator;
        Dictionary<string, RouteInfo> paths = new Dictionary<string, RouteInfo>();
        public Mediator(IApiOrchestrator apiOrchestrator)
        {
            _apiOrchestrator = apiOrchestrator;
        }

        public IMediator AddRoute(string key, RouteInfo routeInfo)
        {
            paths.Add(key.ToLower(), routeInfo);

            return this;
        }

        public IMediator AddRoute(string key, Func<ApiInfo, RouteInfo, HttpRequest, Task<object>> exec)
        {
            paths.Add(key, new RouteInfo
            {                
                Exec = exec
            });

            return this;
        }

        public IApiOrchestrator ToOrchestrator()
        {
            return _apiOrchestrator;
        }

        public RouteInfo GetRoute(string key)
        {
            return paths[key.ToLower()];
        }

    }

}
