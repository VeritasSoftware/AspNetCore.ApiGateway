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

    public class ApiOrchestrator : IApiOrchestrator
    {
        Dictionary<string, ApiInfo> apis = new Dictionary<string, ApiInfo>();

        Dictionary<string, string[]> apiUrls = new Dictionary<string, string[]>();

        public IMediator AddApi(string apiKey, params string[] baseUrls)
        {
            var mediator = new Mediator(this);

            apis.Add(apiKey.ToLower(), new ApiInfo() { BaseUrl = baseUrls.First(), Mediator = mediator });

            apiUrls.Add(apiKey.ToLower(), baseUrls);

            return mediator;
        }

        public ApiInfo GetApi(string apiKey)
        {
            var apiInfo = apis[apiKey.ToLower()];

            var baseUrls = apiUrls[apiKey.ToLower()];

            if (baseUrls.Count() > 1)
            {                
                var random = new Random();
                var selected = random.Next(0, baseUrls.Count() - 1);
                apiInfo.BaseUrl = baseUrls[selected];
            }

            return apiInfo;
        }

        public IEnumerable<Orchestration> Orchestration => apis?.Select(x => new Orchestration
        {
            Api = x.Key,
            Routes = x.Value.Mediator.Routes
        });
    }
}
