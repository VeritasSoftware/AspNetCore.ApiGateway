using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Application.HubFilters
{
    public interface IGatewayHubFilter
    {
        ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext);
        Task OnConnectedAsync(HubLifetimeContext context);
        Task OnDisconnectedAsync(HubLifetimeContext context, Exception exception);
    }
}
