using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway
{
    public interface IMediator
    {
        //IMediator AddRoute(string key,
        //                string path, 
        //                string verb, 
        //                Type responseType, 
        //                Type requestType = null, 
        //                Func<RouteInfo, Task<object>> exec = null);

        IMediator AddRoute(string key, RouteInfo routeInfo);

        IMediator AddRoute(string key, Func<ApiInfo, RouteInfo, HttpRequest, Task<object>> exec);

        RouteInfo GetRoute(string key);

        IApiOrchestrator ToOrchestrator();
    }
}