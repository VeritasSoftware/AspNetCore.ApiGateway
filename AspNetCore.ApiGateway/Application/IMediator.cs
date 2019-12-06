using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway
{
    public interface IMediator
    {
        IMediator AddRoute(string key, GatewayVerb verb, RouteInfo routeInfo);

        IMediator AddRoute(string key, GatewayVerb verb, Func<ApiInfo, RouteInfo, HttpRequest, Task<object>> exec);

        GatewayRouteInfo GetRoute(string key);

        IApiOrchestrator ToOrchestrator();
    }
}