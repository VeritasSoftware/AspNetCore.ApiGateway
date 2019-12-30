using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway
{
    public interface IMediator
    {
        IMediator AddRoute(string key, GatewayVerb verb, RouteInfo routeInfo);

        IMediator AddRoute(string key, GatewayVerb verb, Func<ApiInfo, HttpRequest, Task<object>> exec);

        GatewayRouteInfo GetRoute(string key);

        IMediator AddApi(string apiKey, string baseUrl);

        IEnumerable<Route> Routes { get; }
    }
}