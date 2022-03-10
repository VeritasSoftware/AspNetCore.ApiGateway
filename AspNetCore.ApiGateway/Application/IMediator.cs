using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway
{
    public interface IEventSourceMediator
    {
        IMediator AddApi(string apiKey, params string[] baseUrls);

        IHubMediator AddHub(string apiKey, Func<HubConnectionBuilder, HubConnection> connectionBuilder, string receiveKey = null);

        IEventSourceMediator AddEventSource(string apiKey, Func<object> connectionBuilder, string receiveKey);

        IEventSourceMediator AddRoute(string key, EventSourceRouteInfo routeInfo);

        GatewayEventSourceRouteInfo GetRoute(string key);

        IEnumerable<Route> Routes { get; }

        Dictionary<string, GatewayEventSourceRouteInfo> Paths { get; }
    }

    public interface IHubMediator
    {
        IMediator AddApi(string apiKey, params string[] baseUrls);

        IHubMediator AddHub(string apiKey, Func<HubConnectionBuilder, HubConnection> connectionBuilder, string receiveKey = null);

        IEventSourceMediator AddEventSource(string apiKey, Func<object> connectionBuilder, string routeKey);

        IHubMediator AddRoute(string key, HubRouteInfo routeInfo);

        GatewayHubRouteInfo GetRoute(string key);

        IEnumerable<Route> Routes { get; }

        Dictionary<string, GatewayHubRouteInfo> Paths { get; }
    }

    public interface IMediator
    {
        IMediator AddRoute(string key, GatewayVerb verb, RouteInfo routeInfo);

        IMediator AddRoute(string key, GatewayVerb verb, Func<ApiInfo, HttpRequest, Task<object>> exec);

        GatewayRouteInfo GetRoute(string key);

        IMediator AddApi(string apiKey, params string[] baseUrls);

        IHubMediator AddHub(string apiKey, Func<HubConnectionBuilder, HubConnection> connectionBuilder, string receiveKey = null);

        IEventSourceMediator AddEventSource(string apiKey, Func<object> connectionBuilder, string receiveKey);

        IEnumerable<Route> Routes { get; }
    }
}