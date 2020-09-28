using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway
{
    public interface IHubMediator
    {
        IMediator AddApi(string apiKey, params string[] baseUrls);

        IHubMediator AddHub(string apiKey, Func<HubConnectionBuilder, HubConnection> connectionBuilder);

        IHubMediator AddRoute(string key, HubRouteInfo routeInfo);

        GatewayRouteInfo GetRoute(string key);

        IEnumerable<Route> Routes { get; }

        Dictionary<string, GatewayRouteInfo> Paths { get; }
    }

    public interface IMediator
    {
        IMediator AddRoute(string key, GatewayVerb verb, RouteInfo routeInfo);

        IMediator AddRoute(string key, GatewayVerb verb, Func<ApiInfo, HttpRequest, Task<object>> exec);

        GatewayRouteInfo GetRoute(string key);

        IMediator AddApi(string apiKey, params string[] baseUrls);

        IHubMediator AddHub(string apiKey, Func<HubConnectionBuilder, HubConnection> connectionBuilder);

        IEnumerable<Route> Routes { get; }
    }
}