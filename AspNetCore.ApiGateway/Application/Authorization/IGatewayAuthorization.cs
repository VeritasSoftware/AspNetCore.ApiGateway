using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCore.ApiGateway.Authorization
{
    public interface IGatewayAuthorization
    {
        void Authorize(AuthorizationFilterContext context);
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
