namespace AspNetCore.ApiGateway.Minimal
{
    public interface IGatewayMiddleware
    {
        Task Invoke(HttpContext context, string apiKey, string routeKey);
    }
}
