using System.Collections.Generic;

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

        public IMediator AddApi(string apiKey, string baseUrl)
        {
            var mediator = new Mediator(this);

            apis.Add(apiKey, new ApiInfo() { BaseUrl = baseUrl, Mediator = mediator });

            return mediator;
        }

        public ApiInfo GetApi(string apiKey)
        {
            return apis[apiKey];
        }

    }
}
