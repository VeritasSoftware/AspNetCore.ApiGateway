using AspNetCore.ApiGateway.Application;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Hubs
{
    public class GatewayHub : Hub
    {
        readonly IApiOrchestrator _apiOrchestrator;

        readonly static List<GatewayHubUserExtended> _connectedUsers = new List<GatewayHubUserExtended>();
        private static object _lockObject = new object();

        public GatewayHub(IApiOrchestrator apiOrchestrator)
        {
            _apiOrchestrator = apiOrchestrator;
        }

        public async Task SubscribeToRoute(GatewayHubUser user)
        {
            if (!string.IsNullOrEmpty(user.UserId) && !string.IsNullOrEmpty(user.ReceiveKey) && !string.IsNullOrEmpty(user.Api) && !string.IsNullOrEmpty(user.Key))
            {
                var hubInfo = _apiOrchestrator.GetHub(user.Api);

                var routeInfo = hubInfo.Mediator.GetRoute(user.Key);

                if (!string.IsNullOrEmpty(user.ReceiveKey) && string.Compare(hubInfo.ReceiveKey, user.ReceiveKey) == 0)
                {
                    lock (_lockObject)
                    {
                        if (!_connectedUsers.Any(u => string.Compare(u.UserId, user.UserId, true) == 0))
                        {
                            var extUser = new GatewayHubUserExtended
                            {
                                ConnectionId = Context.ConnectionId,
                                Api = user.Api,
                                Key = user.Key,
                                UserId = user.UserId,
                                ReceiveKey = user.ReceiveKey
                            };

                            _connectedUsers.Add(extUser);
                        }
                    }                                       
                }
            }

            await Task.CompletedTask;
        }
        public async Task UnsubscribeFromRoute(GatewayHubUser user)
        {
            lock (_lockObject)
            {
                _connectedUsers.RemoveAll(u => string.Compare(u.UserId, user.UserId, true) == 0 && string.Compare(u.Api, user.Api, true) == 0 
                                                && string.Compare(u.Key, user.Key, true) == 0 && string.Compare(u.ReceiveKey, user.ReceiveKey) == 0);
            }

            await Task.CompletedTask;
        }

        public async Task Receive(HubReceiveAuth auth, HubRouteInfo route, object[] arg1, object arg2)
        {
            var hubRouteInfo = _apiOrchestrator.GetHub(auth.Api);
            var receiveKey = hubRouteInfo.ReceiveKey;

            if (!string.IsNullOrEmpty(auth.ReceiveKey) && !string.IsNullOrEmpty(receiveKey) && string.Compare(receiveKey, auth.ReceiveKey) == 0)
            {
                IEnumerable<string> connectionIds = null;
                var routeInfo = hubRouteInfo.Mediator.Paths.Single(route => string.Compare(route.Key, auth.Key, true) == 0);

                if (routeInfo.Value.HubRoute.BroadcastType == HubBroadcastType.Group && !string.IsNullOrEmpty(route.ReceiveGroup))
                {
                    await base.Clients.Group(route.ReceiveGroup).SendAsync(route.ReceiveMethod, arg1, arg2);

                    return;
                }
                
                if (routeInfo.Value.HubRoute.BroadcastType != HubBroadcastType.Group && _connectedUsers.Any())
                {
                    connectionIds = _connectedUsers.Where(user => string.Compare(user.Api, auth.Api, true) == 0 && string.Compare(receiveKey, user.ReceiveKey) == 0 
                                                                        && hubRouteInfo.Mediator.Routes.Any(route => string.Compare(route.Key, user.Key, true) == 0))
                                                              .Select(x => x.ConnectionId).ToList();                    
                    
                    if (connectionIds.Any())
                    {
                        foreach (var connId in connectionIds)
                        {
                            await base.Clients.Client(connId).SendAsync(route.ReceiveMethod, arg1, arg2);
                        }
                    }                    
                }
                                
                if (routeInfo.Value.HubRoute.BroadcastType == HubBroadcastType.All)
                {
                    if (connectionIds != null)
                    {
                        await base.Clients.AllExcept(connectionIds).SendAsync(route.ReceiveMethod, arg1, arg2);
                    }
                    else
                    {
                        await base.Clients.All.SendAsync(route.ReceiveMethod, arg1, arg2);
                    }
                }
            }            
        }

    }
}
