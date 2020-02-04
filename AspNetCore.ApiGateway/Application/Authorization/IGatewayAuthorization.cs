using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Authorization
{
    public interface IGatewayAuthorization
    {
        Task AuthorizeAsync(AuthorizationFilterContext context, string api, string key, string verb);
    }

    public interface IGatewayVerbAuthorization
    {
        Task AuthorizeAsync(AuthorizationFilterContext context, string api, string key);
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
