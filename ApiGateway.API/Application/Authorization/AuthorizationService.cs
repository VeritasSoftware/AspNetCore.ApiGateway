using AspNetCore.ApiGateway.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace ApiGateway.API.Application.Authorization
{
    public class GetAuthorizationService : IGetGatewayAuthorization
    {
        public async Task AuthorizeAsync(AuthorizationFilterContext context)
        {
            //Put your authorization here
        }
    }
}
