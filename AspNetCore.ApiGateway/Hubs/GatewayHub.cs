using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Hubs
{
    public class GatewayHub : Hub
    {
        public async Task Receive(HubRouteInfo route, object[] arg1, object arg2)
        {
            await base.Clients.All.SendAsync(route.ReceiveMethod, arg1, arg2);
        }

    }
}
