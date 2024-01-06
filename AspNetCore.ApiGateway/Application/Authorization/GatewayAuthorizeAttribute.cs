using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
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

                routeData.Values.TryGetValue("apiKey", out var apiKey);
                routeData.Values.TryGetValue("routeKey", out var routeKey);

                await this._authorization.AuthorizeAsync(context, apiKey?.ToString(), routeKey?.ToString(), context.HttpContext.Request.Method);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal abstract class GatewayVerbAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        readonly IGatewayVerbAuthorization _authorization;

        public GatewayVerbAuthorizeAttribute(IGatewayVerbAuthorization authorization = null)
        {
            _authorization = authorization;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (this._authorization != null)
            {
                var routeData = context.HttpContext.GetRouteData();

                routeData.Values.TryGetValue("apiKey", out var apiKey);
                routeData.Values.TryGetValue("routeKey", out var routeKey);

                await this._authorization.AuthorizeAsync(context, apiKey?.ToString(), routeKey?.ToString());
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayGetOrHeadAuthorizeAttribute : GatewayVerbAuthorizeAttribute
    {
        public GatewayGetOrHeadAuthorizeAttribute(IGetOrHeadGatewayAuthorization authorization = null) : base(authorization)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayGetOrchestrationAuthorizeAttribute : GatewayVerbAuthorizeAttribute
    {
        public GatewayGetOrchestrationAuthorizeAttribute(IGetOrchestrationGatewayAuthorization authorization = null) : base(authorization)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayPostAuthorizeAttribute : GatewayVerbAuthorizeAttribute
    {
        public GatewayPostAuthorizeAttribute(IPostGatewayAuthorization authorization = null) : base(authorization)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayHubPostAuthorizeAttribute : GatewayVerbAuthorizeAttribute
    {
        public GatewayHubPostAuthorizeAttribute(IHubPostGatewayAuthorization authorization = null) : base(authorization)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayPutAuthorizeAttribute : GatewayVerbAuthorizeAttribute
    {
        public GatewayPutAuthorizeAttribute(IPutGatewayAuthorization authorization = null) : base(authorization)
        {
        }
    }

    internal class GatewayPatchAuthorizeAttribute : GatewayVerbAuthorizeAttribute
    {
        public GatewayPatchAuthorizeAttribute(IPatchGatewayAuthorization authorization = null) : base(authorization)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayDeleteAuthorizeAttribute : GatewayVerbAuthorizeAttribute
    {
        public GatewayDeleteAuthorizeAttribute(IDeleteGatewayAuthorization authorization = null) : base(authorization)
        {
        }
    }
}
