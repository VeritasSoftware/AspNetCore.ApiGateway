using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Application.HubFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal class GatewayHubFilterAttribute : Attribute, IHubFilter
    {
        public GatewayHubFilterAttribute()
        {
        }

        public async ValueTask<object> InvokeMethodAsync(
                    HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object>> next)
        {
            var logger = invocationContext.ServiceProvider.GetServiceOrNull<ILogger<ApiGatewayLog>>();

            logger?.LogInformation($"Calling hub method '{invocationContext.HubMethodName}'");

            var gatewayHubFilter = invocationContext.ServiceProvider.GetServiceOrNull<IGatewayHubFilter>();

            if(gatewayHubFilter != null)
                await gatewayHubFilter.InvokeMethodAsync(invocationContext);

            try
            {
                return await next(invocationContext);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Exception calling '{invocationContext.HubMethodName}'");
                throw;
            }
        }
        public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
        {
            var logger = context.ServiceProvider.GetServiceOrNull<ILogger<ApiGatewayLog>>();

            logger?.LogInformation($"Connected connection id: '{context.Context.ConnectionId}'");

            var gatewayHubFilter = context.ServiceProvider.GetServiceOrNull<IGatewayHubFilter>();

            if (gatewayHubFilter != null)
                await gatewayHubFilter.OnConnectedAsync(context);

            await next(context);
        }

        public async Task OnDisconnectedAsync(
            HubLifetimeContext context, Exception exception, Func<HubLifetimeContext, Exception, Task> next)
        {
            var logger = context.ServiceProvider.GetServiceOrNull<ILogger<ApiGatewayLog>>();

            logger?.LogInformation($"Disconnected connection id: '{context.Context.ConnectionId}'");

            var gatewayHubFilter = context.ServiceProvider.GetServiceOrNull<IGatewayHubFilter>();

            if (gatewayHubFilter != null)
                await gatewayHubFilter.OnDisconnectedAsync(context, exception);

            await next(context, exception);
        }
    }
}
