using System.Collections.Generic;

namespace AspNetCore.ApiGateway
{
    public interface IApiOrchestrator
    {
        IMediator AddApi(string apiKey, string baseUrl);

        ApiInfo GetApi(string apiKey);

        IEnumerable<Orchestration> Orchestration { get; }
    }
}