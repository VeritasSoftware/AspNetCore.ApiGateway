using System.Collections.Generic;

namespace AspNetCore.ApiGateway
{
    public interface IApiOrchestrator
    {
        IMediator AddApi(string apiKey, params string[] baseUrl);

        ApiInfo GetApi(string apiKey);

        IEnumerable<Orchestration> Orchestration { get; }
    }
}