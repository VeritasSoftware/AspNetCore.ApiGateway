using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Middleware
{
    public interface IGatewayMiddleware
    {
        Task Invoke(HttpContext context, string apiKey, string routeKey);
    }
}
