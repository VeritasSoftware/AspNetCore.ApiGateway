using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;

namespace AspNetCore.ApiGateway
{
    public interface IApiOrchestrator
    {
        IMediator AddApi(string apiKey, params string[] baseUrl);
        IMediator AddApi(string apiKey, LoadBalancingType loadBalancingType, params string[] baseUrls);

        IHubMediator AddHub(string apiKey, Func<HubConnectionBuilder, HubConnection> connectionBuilder, string receiveKey = null);

        IEventSourceMediator AddEventSource(string apiKey, Func<object> connectionBuilder, string routeKey);

        ApiInfo GetApi(string apiKey, bool withLoadBalancing = false);

        HubInfo GetHub(string apiKey);
        EventSourcingInfo GetEventSource(string apiKey);

        IEnumerable<Orchestration> Orchestration { get; }

        Dictionary<string, HubInfo> Hubs { get; }

        string GatewayHubUrl { get; set; }

        public bool StartGatewayHub { get; set; }
    }
}