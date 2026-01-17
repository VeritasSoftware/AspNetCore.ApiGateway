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

        ApiInfo GetApi(string apiKey, bool withLoadBalancing = false);

        HubInfo GetHub(string apiKey);

        IEnumerable<Orchestration> Orchestration { get; }

        Dictionary<string, HubInfo> Hubs { get; }

        string GatewayHubUrl { get; set; }

        public bool StartGatewayHub { get; set; }
    }
}