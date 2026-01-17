using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway
{
    public interface IMediatorBase
    {
        IApiOrchestrator ApiOrchestrator { get; set; }
    }

    public interface IHubMediator: IMediatorBase
    {
        IMediator AddApi(string apiKey, params string[] baseUrls);

        IHubMediator AddHub(string apiKey, Func<HubConnectionBuilder, HubConnection> connectionBuilder, string receiveKey = null);

        IHubMediator AddRoute(string routeKey, HubRouteInfo routeInfo);

        GatewayHubRouteInfo GetRoute(string routeKey);

        IEnumerable<HubRoute> Routes { get; }

        Dictionary<string, GatewayHubRouteInfo> Paths { get; }
    }

    public interface IMediator: IMediatorBase
    {
        IMediator AddRoute(string routeKey, GatewayVerb verb, RouteInfo routeInfo);

        IMediator AddRoute(string routeKey, GatewayVerb verb, Func<ApiInfo, HttpRequest, Task<object>> exec);

        GatewayRouteInfo GetRoute(string routeKey);

        IMediator AddApi(string apiKey, params string[] baseUrls);

        IHubMediator AddHub(string apiKey, Func<HubConnectionBuilder, HubConnection> connectionBuilder, string receiveKey = null);

        IEnumerable<Route> Routes { get; }
    }
}