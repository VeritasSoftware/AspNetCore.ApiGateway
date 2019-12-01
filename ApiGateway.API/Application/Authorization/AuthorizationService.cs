using AspNetCore.ApiGateway;
using AspNetCore.ApiGateway.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace ApiGateway.API.Application.Authorization
{
    public class GetAuthorizationService : IGetGatewayAuthorization
    {
        public async Task AuthorizeAsync(AuthorizationFilterContext context, string api, string key)
        {
            //Put your authorization here
        }
    }
}
