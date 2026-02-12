namespace AspNetCore.ApiGateway.Minimal
{
    public interface IApiOrchestrator
    {
        IMediator AddApi(string apiKey, params string[] baseUrl);
        IMediator AddApi(string apiKey, LoadBalancingType loadBalancingType, params string[] baseUrls);

        ApiInfo GetApi(string apiKey, bool withLoadBalancing = false);

        IEnumerable<Orchestration> Orchestration { get; }
    }
}