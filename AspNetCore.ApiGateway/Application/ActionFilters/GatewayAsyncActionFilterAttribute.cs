﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Application.ActionFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayAsyncActionFilterAttribute : Attribute, IAsyncActionFilter
    {
        readonly IGatewayActionFilter _gatewayActionFilter;

        public GatewayAsyncActionFilterAttribute(IGatewayActionFilter gatewayActionFilter = null)
        {
            _gatewayActionFilter = gatewayActionFilter;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (_gatewayActionFilter != null)
            {
                var routeData = context.HttpContext.GetRouteData();

                routeData.Values.TryGetValue("api", out var api);
                routeData.Values.TryGetValue("key", out var key);

                await _gatewayActionFilter.OnActionExecutionAsync(context, api?.ToString(), key?.ToString(), context.HttpContext.Request.Method);
            }

            if (context.Result == null)
                await next();
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal abstract class GatewayVerbAsyncActionFilterAttribute : Attribute, IAsyncActionFilter
    {
        readonly IGatewayVerbActionFilter _gatewayActionFilter;

        public GatewayVerbAsyncActionFilterAttribute(IGatewayVerbActionFilter gatewayActionFilter = null)
        {
            _gatewayActionFilter = gatewayActionFilter;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (_gatewayActionFilter != null)
            {
                var routeData = context.HttpContext.GetRouteData();

                routeData.Values.TryGetValue("api", out var api);
                routeData.Values.TryGetValue("key", out var key);

                await _gatewayActionFilter.OnActionExecutionAsync(context, api?.ToString(), key?.ToString());
            }

            if (context.Result == null)
                await next();
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayGetOrHeadAsyncActionFilterAttribute : GatewayVerbAsyncActionFilterAttribute
    {
        public GatewayGetOrHeadAsyncActionFilterAttribute(IGetOrHeadGatewayActionFilter actionFilter = null) : base(actionFilter)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayPostAsyncActionFilterAttribute : GatewayVerbAsyncActionFilterAttribute
    {
        public GatewayPostAsyncActionFilterAttribute(IPostGatewayActionFilter actionFilter = null) : base(actionFilter)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayPutAsyncActionFilterAttribute : GatewayVerbAsyncActionFilterAttribute
    {
        public GatewayPutAsyncActionFilterAttribute(IPutGatewayActionFilter actionFilter = null) : base(actionFilter)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayPatchAsyncActionFilterAttribute : GatewayVerbAsyncActionFilterAttribute
    {
        public GatewayPatchAsyncActionFilterAttribute(IPatchGatewayActionFilter actionFilter = null) : base(actionFilter)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayDeleteAsyncActionFilterAttribute : GatewayVerbAsyncActionFilterAttribute
    {
        public GatewayDeleteAsyncActionFilterAttribute(IDeleteGatewayActionFilter actionFilter = null) : base(actionFilter)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayGetOrchestrationAsyncActionFilterAttribute : GatewayVerbAsyncActionFilterAttribute
    {
        public GatewayGetOrchestrationAsyncActionFilterAttribute(IGetOrchestrationGatewayActionFilter actionFilter = null) : base(actionFilter)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayHubPostAsyncActionFilterAttribute : GatewayVerbAsyncActionFilterAttribute
    {
        public GatewayHubPostAsyncActionFilterAttribute(IHubPostGatewayActionFilter actionFilter = null) : base(actionFilter)
        {
        }
    }
}
