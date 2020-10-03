using AspNetCore.ApiGateway.Application;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Hubs
{
    public class GatewayHub : Hub
    {
        readonly IApiOrchestrator _apiOrchestrator;

        public GatewayHub(IApiOrchestrator apiOrchestrator)
        {
            _apiOrchestrator = apiOrchestrator;
        }

        public async Task Receive(HubReceiveAuth auth, HubRouteInfo route, object[] arg1, object arg2)
        {
            var hubRouteInfo = _apiOrchestrator.GetHub(auth.Hub);
            var receiveKey = hubRouteInfo.ReceiveKey;

            if (string.Compare(receiveKey, auth.ReceiveKey) == 0)
            {
                await base.Clients.All.SendAsync(route.ReceiveMethod, arg1, arg2);
            }            
        }

    }
}
