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

    public interface IEventSourceMediator : IMediatorBase
    {
        IMediator AddApi(string apiKey, params string[] baseUrls);

        IHubMediator AddHub(string apiKey, Func<HubConnectionBuilder, HubConnection> connectionBuilder, string receiveKey = null);

        IEventSourceMediator AddEventSource(string apiKey, Func<object> connectionBuilder, string receiveKey);

        IEventSourceMediator AddRoute(string routeKey, EventSourceRouteInfo routeInfo);

        GatewayEventSourceRouteInfo GetRoute(string routeKey);

        IEnumerable<EventSourceRoute> Routes { get; }

        Dictionary<string, GatewayEventSourceRouteInfo> Paths { get; }
    }

    public interface IHubMediator: IMediatorBase
    {
        IMediator AddApi(string apiKey, params string[] baseUrls);

        IHubMediator AddHub(string apiKey, Func<HubConnectionBuilder, HubConnection> connectionBuilder, string receiveKey = null);

        IEventSourceMediator AddEventSource(string apiKey, Func<object> connectionBuilder, string routeKey);

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

        IEventSourceMediator AddEventSource(string apiKey, Func<object> connectionBuilder, string receiveKey);

        IEnumerable<Route> Routes { get; }
    }
}