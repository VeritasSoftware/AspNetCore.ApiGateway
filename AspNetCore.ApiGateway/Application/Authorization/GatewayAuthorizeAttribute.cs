using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal abstract class GatewayAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        readonly IGatewayAuthorization _authorization;

        public GatewayAuthorizeAttribute(IGatewayAuthorization authorization = null)
        {
            _authorization = authorization;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (this._authorization != null)
            {
                var routeData = context.HttpContext.GetRouteData();

                routeData.Values.TryGetValue("api", out var api);
                routeData.Values.TryGetValue("key", out var key);

                await this._authorization.AuthorizeAsync(context, api.ToString(), key.ToString());
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayGetAuthorizeAttribute : GatewayAuthorizeAttribute
    {
        public GatewayGetAuthorizeAttribute(IGetGatewayAuthorization authorization = null) : base(authorization)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayPostAuthorizeAttribute : GatewayAuthorizeAttribute
    {
        public GatewayPostAuthorizeAttribute(IPostGatewayAuthorization authorization = null) : base(authorization)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayPutAuthorizeAttribute : GatewayAuthorizeAttribute
    {
        public GatewayPutAuthorizeAttribute(IPutGatewayAuthorization authorization = null) : base(authorization)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayDeleteAuthorizeAttribute : GatewayAuthorizeAttribute
    {
        public GatewayDeleteAuthorizeAttribute(IDeleteGatewayAuthorization authorization = null) : base(authorization)
        {
        }
    }
}
