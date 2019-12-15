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

    public interface IGetGatewayAuthorization : IGatewayVerbAuthorization
    {
    }

    public interface IPostGatewayAuthorization : IGatewayVerbAuthorization
    {
    }

    public interface IPutGatewayAuthorization : IGatewayVerbAuthorization
    {
    }

    public interface IDeleteGatewayAuthorization : IGatewayVerbAuthorization
    {
    }
}
