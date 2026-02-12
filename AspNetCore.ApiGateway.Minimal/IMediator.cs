namespace AspNetCore.ApiGateway.Minimal
{
    public interface IMediatorBase
    {
        IApiOrchestrator ApiOrchestrator { get; set; }
    }

    public interface IMediator: IMediatorBase
    {
        IMediator AddRoute(string routeKey, GatewayVerb verb, RouteInfo routeInfo);

        IMediator AddRoute(string routeKey, GatewayVerb verb, Func<ApiInfo, HttpRequest, Task<object>> exec);

        GatewayRouteInfo GetRoute(string routeKey);

        IMediator AddApi(string apiKey, params string[] baseUrls);

        IEnumerable<Route> Routes { get; }
    }
}