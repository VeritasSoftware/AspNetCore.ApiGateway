using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Authorization
{
    public interface IGatewayAuthorization
    {
        Task AuthorizeAsync(AuthorizationFilterContext context, string api, string key);
    }

    public interface IGetGatewayAuthorization : IGatewayAuthorization
    {
    }

    public interface IPostGatewayAuthorization : IGatewayAuthorization
    {
    }

    public interface IPutGatewayAuthorization : IGatewayAuthorization
    {
    }

    public interface IDeleteGatewayAuthorization : IGatewayAuthorization
    {
    }
}
