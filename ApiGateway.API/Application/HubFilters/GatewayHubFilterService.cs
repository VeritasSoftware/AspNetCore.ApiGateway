using AspNetCore.ApiGateway.Application.HubFilters;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ApiGateway.API.Application.HubFilters
{
    public class GatewayHubFilterService : IGatewayHubFilter
    {
        public ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext)
        {
            //Do your work here eg.
            //invocationContext.Context.Abort();
            return new ValueTask<object>();
        }

        public Task OnConnectedAsync(HubLifetimeContext context)
        {
            //Do your work here
            return Task.CompletedTask;
        }

        public Task OnDisconnectedAsync(HubLifetimeContext context, Exception exception)
        {
            //Do your work here
            return Task.CompletedTask;
        }
    }
}
