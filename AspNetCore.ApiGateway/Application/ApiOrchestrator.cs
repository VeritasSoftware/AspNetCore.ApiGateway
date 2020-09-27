using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.ApiGateway
{
    public class ApiInfo
    {
        public string BaseUrl { get; set; }

        public Mediator Mediator { get; set; }
    }

    public class HubInfo
    {
        public string BaseUrl { get; set; }

        public HubMediator Mediator { get; set; }

        public HubConnection Connection { get; set; }
    }

    public class ApiOrchestrator : IApiOrchestrator
    {
        Dictionary<string, ApiInfo> apis = new Dictionary<string, ApiInfo>();

        Dictionary<string, HubInfo> hubs = new Dictionary<string, HubInfo>();

        Dictionary<string, string[]> apiUrls = new Dictionary<string, string[]>();

        public IMediator AddApi(string apiKey, params string[] baseUrls)
        {
            var mediator = new Mediator(this);

            apis.Add(apiKey.ToLower(), new ApiInfo() { BaseUrl = baseUrls.First(), Mediator = mediator });

            apiUrls.Add(apiKey.ToLower(), baseUrls);

            return mediator;
        }

        public IHubMediator AddHub(string apiKey, Func<HubConnectionBuilder, HubConnection> connectionBuilder)
        {
            var mediator = new HubMediator(this);

            var cb = new HubConnectionBuilder();

            var conn = connectionBuilder(cb);            

            hubs.Add(apiKey.ToLower(), new HubInfo() { Mediator = mediator, Connection = conn });

            return mediator;
        }

        public ApiInfo GetApi(string apiKey)
        {
            var apiInfo = apis[apiKey.ToLower()];

            var baseUrls = apiUrls[apiKey.ToLower()];

            //if more than 1 base url is specified
            //get the load balancing base url based on Random selection
            if (baseUrls.Count() > 1)
            {
                apiInfo.BaseUrl = this.GetLoadBalancingUrl(baseUrls);
            }

            return apiInfo;
        }

        public HubInfo GetHub(string apiKey)
        {
            var apiInfo = hubs[apiKey.ToLower()];

            return apiInfo;
        }

        public IEnumerable<Orchestration> Orchestration => apis?.Select(x => new Orchestration
        {
            Api = x.Key,
            Routes = x.Value.Mediator.Routes
        }).Union(hubs?.Select(x => new Orchestration
        {
            Api = x.Key,
            Routes = x.Value.Mediator.Routes
        }));

        private string GetLoadBalancingUrl(string[] baseUrls)
        {
            var random = new Random();
            var selected = random.Next(0, baseUrls.Count() - 1);
            return baseUrls[selected];
        }
    }
}
