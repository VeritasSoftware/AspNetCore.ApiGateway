using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;

namespace AspNetCore.ApiGateway
{
    public interface IApiOrchestrator
    {
        IMediator AddApi(string apiKey, params string[] baseUrl);

        IHubMediator AddHub(string apiKey, Func<HubConnectionBuilder, HubConnection> connectionBuilder, string receiveKey = null);

        IEventSourceMediator AddEventSource(string apiKey, Func<object> connectionBuilder, string receiveKey);

        ApiInfo GetApi(string apiKey);

        HubInfo GetHub(string apiKey);
        EventSourcingInfo GetEventSource(string apiKey);

        IEnumerable<Orchestration> Orchestration { get; }

        Dictionary<string, HubInfo> Hubs { get; }

        string GatewayHubUrl { get; set; }

        public bool StartGatewayHub { get; set; }
    }
}