using AspNetCore.ApiGateway.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace ApiGateway.API.Application.Authorization
{
    public class AuthorizationService : IGatewayAuthorization
    {
        public async Task AuthorizeAsync(AuthorizationFilterContext context, string apiKey, string routeKey, string verb)
        {
            //Put your authorization here

            await Task.CompletedTask;
        }
    }

    public class GetAuthorizationService : IGetOrHeadGatewayAuthorization
    {
        public async Task AuthorizeAsync(AuthorizationFilterContext context, string apiKey, string routeKey)
        {
            //Put your authorization here

            await Task.CompletedTask;
        }
    }
}
