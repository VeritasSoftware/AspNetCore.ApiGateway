using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Application.ExceptionFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayAsyncExceptionFilterAttribute : Attribute, IAsyncExceptionFilter
    {
        readonly IGatewayExceptionFilter _gatewayExceptionFilter;

        public GatewayAsyncExceptionFilterAttribute(IGatewayExceptionFilter gatewayExceptionFilter = null)
        {
            _gatewayExceptionFilter = gatewayExceptionFilter;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            if(_gatewayExceptionFilter != null)
            {
                var routeData = context.HttpContext.GetRouteData();

                routeData.Values.TryGetValue("apiKey", out var apiKey);
                routeData.Values.TryGetValue("routeKey", out var routeKey);

                await _gatewayExceptionFilter.OnExceptionAsync(context, apiKey?.ToString(), routeKey?.ToString(), context.HttpContext.Request.Method);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayVerbAsyncExceptionFilterAttribute : Attribute, IAsyncExceptionFilter
    {
        readonly IGatewayVerbExceptionFilter _gatewayExceptionFilter;

        public GatewayVerbAsyncExceptionFilterAttribute(IGatewayVerbExceptionFilter gatewayExceptionFilter = null)
        {
            _gatewayExceptionFilter = gatewayExceptionFilter;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            if (_gatewayExceptionFilter != null)
            {
                var routeData = context.HttpContext.GetRouteData();

                routeData.Values.TryGetValue("apiKey", out var apiKey);
                routeData.Values.TryGetValue("routeKey", out var routeKey);

                await _gatewayExceptionFilter.OnExceptionAsync(context, apiKey?.ToString(), routeKey?.ToString());
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayGetOrHeadAsyncExceptionFilterAttribute : GatewayVerbAsyncExceptionFilterAttribute
    {
        public GatewayGetOrHeadAsyncExceptionFilterAttribute(IGetOrHeadGatewayExceptionFilter ExceptionFilter = null) : base(ExceptionFilter)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayPostAsyncExceptionFilterAttribute : GatewayVerbAsyncExceptionFilterAttribute
    {
        public GatewayPostAsyncExceptionFilterAttribute(IPostGatewayExceptionFilter ExceptionFilter = null) : base(ExceptionFilter)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayPutAsyncExceptionFilterAttribute : GatewayVerbAsyncExceptionFilterAttribute
    {
        public GatewayPutAsyncExceptionFilterAttribute(IPutGatewayExceptionFilter ExceptionFilter = null) : base(ExceptionFilter)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayPatchAsyncExceptionFilterAttribute : GatewayVerbAsyncExceptionFilterAttribute
    {
        public GatewayPatchAsyncExceptionFilterAttribute(IPatchGatewayExceptionFilter ExceptionFilter = null) : base(ExceptionFilter)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayDeleteAsyncExceptionFilterAttribute : GatewayVerbAsyncExceptionFilterAttribute
    {
        public GatewayDeleteAsyncExceptionFilterAttribute(IDeleteGatewayExceptionFilter ExceptionFilter = null) : base(ExceptionFilter)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayGetOrchestrationAsyncExceptionFilterAttribute : GatewayVerbAsyncExceptionFilterAttribute
    {
        public GatewayGetOrchestrationAsyncExceptionFilterAttribute(IGetOrchestrationGatewayExceptionFilter ExceptionFilter = null) : base(ExceptionFilter)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayHubPostAsyncExceptionFilterAttribute : GatewayVerbAsyncExceptionFilterAttribute
    {
        public GatewayHubPostAsyncExceptionFilterAttribute(IHubPostGatewayExceptionFilter ExceptionFilter = null) : base(ExceptionFilter)
        {
        }
    }
}
