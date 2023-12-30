using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Authorization
{
    public interface IGatewayAuthorization
    {
        Task AuthorizeAsync(AuthorizationFilterContext context, string apiKey, string routeKey, string verb);
    }

    public interface IGatewayVerbAuthorization
    {
        Task AuthorizeAsync(AuthorizationFilterContext context, string apiKey, string routeKey);
    }

    public interface IGetOrHeadGatewayAuthorization : IGatewayVerbAuthorization
    {
    }

    public interface IGetOrchestrationGatewayAuthorization : IGatewayVerbAuthorization
    {
    }

    public interface IPostGatewayAuthorization : IGatewayVerbAuthorization
    {
    }

    public interface IHubPostGatewayAuthorization : IGatewayVerbAuthorization
    {
    }

    public interface IPutGatewayAuthorization : IGatewayVerbAuthorization
    {
    }

    public interface IPatchGatewayAuthorization : IGatewayVerbAuthorization
    {
    }

    public interface IDeleteGatewayAuthorization : IGatewayVerbAuthorization
    {
    }
}
