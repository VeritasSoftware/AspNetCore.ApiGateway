using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Application.ResultFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayAsyncResultFilterAttribute : Attribute, IAsyncResultFilter
    {
        readonly IGatewayResultFilter _gatewayResultFilter;

        public GatewayAsyncResultFilterAttribute(IGatewayResultFilter gatewayResultFilter = null)
        {
            _gatewayResultFilter = gatewayResultFilter;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if(_gatewayResultFilter != null)
            {
                var routeData = context.HttpContext.GetRouteData();

                routeData.Values.TryGetValue("apiKey", out var apiKey);
                routeData.Values.TryGetValue("routeKey", out var routeKey);

                await _gatewayResultFilter.OnResultExecutionAsync(context, apiKey?.ToString(), routeKey?.ToString(), context.HttpContext.Request.Method);
            }

            if (!(context.Result is EmptyResult))
            {
                await next();
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayVerbAsyncResultFilterAttribute : Attribute, IAsyncResultFilter
    {
        readonly IGatewayVerbResultFilter _gatewayResultFilter;

        public GatewayVerbAsyncResultFilterAttribute(IGatewayVerbResultFilter gatewayResultFilter = null)
        {
            _gatewayResultFilter = gatewayResultFilter;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (_gatewayResultFilter != null)
            {
                var routeData = context.HttpContext.GetRouteData();

                routeData.Values.TryGetValue("apiKey", out var apiKey);
                routeData.Values.TryGetValue("routeKey", out var routeKey);

                await _gatewayResultFilter.OnResultExecutionAsync(context, apiKey?.ToString(), routeKey?.ToString());
            }

            if (!(context.Result is EmptyResult))
            {
                await next();
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayGetOrHeadAsyncResultFilterAttribute : GatewayVerbAsyncResultFilterAttribute
    {
        public GatewayGetOrHeadAsyncResultFilterAttribute(IGetOrHeadGatewayResultFilter resultFilter = null) : base(resultFilter)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayPostAsyncResultFilterAttribute : GatewayVerbAsyncResultFilterAttribute
    {
        public GatewayPostAsyncResultFilterAttribute(IPostGatewayResultFilter resultFilter = null) : base(resultFilter)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayPutAsyncResultFilterAttribute : GatewayVerbAsyncResultFilterAttribute
    {
        public GatewayPutAsyncResultFilterAttribute(IPutGatewayResultFilter resultFilter = null) : base(resultFilter)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayPatchAsyncResultFilterAttribute : GatewayVerbAsyncResultFilterAttribute
    {
        public GatewayPatchAsyncResultFilterAttribute(IPatchGatewayResultFilter resultFilter = null) : base(resultFilter)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayDeleteAsyncResultFilterAttribute : GatewayVerbAsyncResultFilterAttribute
    {
        public GatewayDeleteAsyncResultFilterAttribute(IDeleteGatewayResultFilter resultFilter = null) : base(resultFilter)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayGetOrchestrationAsyncResultFilterAttribute : GatewayVerbAsyncResultFilterAttribute
    {
        public GatewayGetOrchestrationAsyncResultFilterAttribute(IGetOrchestrationGatewayResultFilter resultFilter = null) : base(resultFilter)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayHubPostAsyncResultFilterAttribute : GatewayVerbAsyncResultFilterAttribute
    {
        public GatewayHubPostAsyncResultFilterAttribute(IHubPostGatewayResultFilter resultFilter = null) : base(resultFilter)
        {
        }
    }
}
