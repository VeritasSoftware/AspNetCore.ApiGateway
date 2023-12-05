using AspNetCore.ApiGateway.Application;
using AspNetCore.ApiGateway.Application.HubFilters;
using EventStore.ClientAPI;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Hubs
{
    [GatewayHubFilter]
    public class GatewayHub : Hub
    {
        readonly IApiOrchestrator _apiOrchestrator;
        readonly ILogger<ApiGatewayLog> _logger;

        readonly static List<GatewayHubUserExtended> _connectedUsers = new List<GatewayHubUserExtended>();
        private static object _lockObject = new object();

        public GatewayHub(IApiOrchestrator apiOrchestrator, ILogger<ApiGatewayLog> logger = null)
        {
            _apiOrchestrator = apiOrchestrator;
            _logger = logger;
        }

        public async Task SubscribeToGroup(GatewayHubGroupUser user)
        {
            if (!string.IsNullOrEmpty(user.ReceiveGroup) && !string.IsNullOrEmpty(user.ReceiveKey) && !string.IsNullOrEmpty(user.Api) && !string.IsNullOrEmpty(user.Key))
            {
                var hubInfo = _apiOrchestrator.GetHub(user.Api);

                var routeInfo = hubInfo.Mediator.GetRoute(user.Key);

                if (routeInfo.HubRoute.BroadcastType == HubBroadcastType.Group && !string.IsNullOrEmpty(user.ReceiveKey) && string.Compare(hubInfo.ReceiveKey, user.ReceiveKey) == 0)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, user.ReceiveGroup);
                }
            }
        }

        public async Task UnsubscribeFromGroup(GatewayHubGroupUser user)
        {
            if (!string.IsNullOrEmpty(user.ReceiveGroup) && !string.IsNullOrEmpty(user.ReceiveKey) && !string.IsNullOrEmpty(user.Api) && !string.IsNullOrEmpty(user.Key))
            {
                var hubInfo = _apiOrchestrator.GetHub(user.Api);

                var routeInfo = hubInfo.Mediator.GetRoute(user.Key);

                if (!string.IsNullOrEmpty(user.ReceiveKey) && string.Compare(hubInfo.ReceiveKey, user.ReceiveKey) == 0)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, user.ReceiveGroup);
                }
            }
        }

        public async Task InvokeDownstreamHub(GatewayHubDownstreamHubUser user)
        {
            if (!string.IsNullOrEmpty(user.ReceiveKey) && !string.IsNullOrEmpty(user.Api) && !string.IsNullOrEmpty(user.Key))
            {
                var hubInfo = _apiOrchestrator.GetHub(user.Api);

                var routeInfo = hubInfo.Mediator.GetRoute(user.Key);

                if (!string.IsNullOrEmpty(user.ReceiveKey) && string.Compare(hubInfo.ReceiveKey, user.ReceiveKey) == 0)
                {
                    var connection = hubInfo.Connection;

                    if (connection.State != HubConnectionState.Connected)
                        await connection.StartAsync();

                    if (user.Data != null)
                    {
                        await connection.InvokeAsync(routeInfo.HubRoute.InvokeMethod, user.Data);
                    }
                    else
                    {
                        await connection.InvokeCoreAsync(routeInfo.HubRoute.InvokeMethod, user.DataArray);
                    }                    
                }
            }
        }

        public async Task PublishToEventStoreStream(GatewayHubEventStoreUser user)
        {
            if (!string.IsNullOrEmpty(user.RouteKey) && !string.IsNullOrEmpty(user.Api) && !string.IsNullOrEmpty(user.Key))
            {
                var eventSourceInfo = _apiOrchestrator.GetEventSource(user.Api);

                var routeInfo = eventSourceInfo.Mediator.GetRoute(user.Key);

                if ((routeInfo.EventSourceRoute.OperationType == EventSourcingOperationType.PublishOnly || routeInfo.EventSourceRoute.OperationType == EventSourcingOperationType.PublishSubscribe) 
                    && (!string.IsNullOrEmpty(user.RouteKey) && string.Compare(eventSourceInfo.RouteKey, user.RouteKey) == 0))
                {
                    var connection = (IEventStoreConnection) eventSourceInfo.Connection;

                    _logger?.LogInformation($"Start publishing Events to downstream Event Store stream {routeInfo.EventSourceRoute.StreamName}, for ConnectionId {this.Context.ConnectionId}.");

                    await connection.AppendToStreamAsync(routeInfo.EventSourceRoute.StreamName, ExpectedVersion.StreamExists, user.Events);

                    _logger?.LogInformation($"Start publishing Events to downstream Event Store stream {routeInfo.EventSourceRoute.StreamName}, for ConnectionId {this.Context.ConnectionId}.");
                }
            }
        }

        public async Task SubscribeToEventStoreStream(GatewayHubSubscribeEventStoreUser user)
        {
            if (!string.IsNullOrEmpty(user.RouteKey) && !string.IsNullOrEmpty(user.Api) && !string.IsNullOrEmpty(user.Key))
            {
                var eventSourceInfo = _apiOrchestrator.GetEventSource(user.Api);

                var routeInfo = eventSourceInfo.Mediator.GetRoute(user.Key);

                if ((routeInfo.EventSourceRoute.OperationType == EventSourcingOperationType.SubscribeOnly || routeInfo.EventSourceRoute.OperationType == EventSourcingOperationType.PublishSubscribe) 
                    && (!string.IsNullOrEmpty(user.RouteKey) && string.Compare(eventSourceInfo.RouteKey, user.RouteKey) == 0))
                {
                    var connection = (IEventStoreConnection)eventSourceInfo.Connection;

                    _logger?.LogInformation($"Start subscribing to downstream Event Store route {user.Api}:{user.Key} for ConnectionId {this.Context.ConnectionId}.");

                    await EventStoreClientFactory.CreateAsync(new EventStoreSubscriptionClientSettings
                    {
                        Connection = connection,
                        RouteInfo = routeInfo.EventSourceRoute,
                        GatewayUrl = _apiOrchestrator.GatewayHubUrl,
                        StoreUser = user,
                        ConnectionId = this.Context.ConnectionId,
                        Logger = _logger
                    });

                    _logger?.LogInformation($"Finish subscribing to downstream Event Store route {user.Api}:{user.Key} for ConnectionId {this.Context.ConnectionId}.");
                }
            }
        }

        public async Task UnsubscribeFromEventStoreStream(GatewayHubSubscribeEventStoreUser user)
        {
            if (!string.IsNullOrEmpty(user.RouteKey) && !string.IsNullOrEmpty(user.Api) && !string.IsNullOrEmpty(user.Key))
            {
                var eventSourceInfo = _apiOrchestrator.GetEventSource(user.Api);

                var routeInfo = eventSourceInfo.Mediator.GetRoute(user.Key);

                if (!string.IsNullOrEmpty(user.RouteKey) && string.Compare(eventSourceInfo.RouteKey, user.RouteKey) == 0)
                {
                    lock(this)
                    {
                        _logger?.LogInformation($"Start unsubscribing to downstream Event Store route {user.Api}:{user.Key} for ConnectionId {this.Context.ConnectionId}.");

                        EventStoreClientFactory.ConnectedUsers.RemoveAll(x => (x.ConnectionId == this.Context.ConnectionId)
                                                                                && (x.StoreUser.Api == user.Api) && (x.StoreUser.Key == user.Key));

                        _logger?.LogInformation($"Finish unsubscribing to downstream Event Store route {user.Api}:{user.Key} for ConnectionId {this.Context.ConnectionId}.");
                    }                    
                }
            }

            await Task.CompletedTask;
        }

        public async Task EventStoreEventAppeared(GatewayHubSubscribeEventStoreUser user, string resolvedEvent)
        {
            if (!string.IsNullOrEmpty(user.RouteKey) && !string.IsNullOrEmpty(user.Api) && !string.IsNullOrEmpty(user.Key))
            {
                var eventSourceInfo = _apiOrchestrator.GetEventSource(user.Api);

                var routeInfo = eventSourceInfo.Mediator.GetRoute(user.Key);

                if (!string.IsNullOrEmpty(user.RouteKey) && string.Compare(eventSourceInfo.RouteKey, user.RouteKey) == 0)
                {
                    var connectedUsers = EventStoreClientFactory.ConnectedUsers.Where(x => (x.StoreUser.Api == user.Api) && (x.StoreUser.Key == user.Key)).ToList();

                    connectedUsers.ForEach(async connectedUser =>
                    {
                        _logger?.LogInformation($"Start sending Event to ConnectionId {connectedUser.ConnectionId}.");

                        await base.Clients.Client(connectedUser.ConnectionId).SendAsync(routeInfo.EventSourceRoute.ReceiveMethod, resolvedEvent, new object());

                        _logger?.LogInformation($"Finish sending Event to ConnectionId {connectedUser.ConnectionId}.");
                    });                    
                }
            }

            await Task.CompletedTask;
        }

        public async Task SubscribeToRoute(GatewayHubUser user)
        {
            if (!string.IsNullOrEmpty(user.UserId) && !string.IsNullOrEmpty(user.ReceiveKey) && !string.IsNullOrEmpty(user.Api) && !string.IsNullOrEmpty(user.Key))
            {
                var hubInfo = _apiOrchestrator.GetHub(user.Api);

                var routeInfo = hubInfo.Mediator.GetRoute(user.Key);

                if (routeInfo.HubRoute.BroadcastType == HubBroadcastType.Individual && !string.IsNullOrEmpty(user.ReceiveKey) && string.Compare(hubInfo.ReceiveKey, user.ReceiveKey) == 0)
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
            if (!string.IsNullOrEmpty(user.UserId) && !string.IsNullOrEmpty(user.ReceiveKey) && !string.IsNullOrEmpty(user.Api) && !string.IsNullOrEmpty(user.Key))
            {
                var hubInfo = _apiOrchestrator.GetHub(user.Api);

                var routeInfo = hubInfo.Mediator.GetRoute(user.Key);

                if (!string.IsNullOrEmpty(user.ReceiveKey) && string.Compare(hubInfo.ReceiveKey, user.ReceiveKey) == 0)
                {
                    lock (_lockObject)
                    {
                        _connectedUsers.RemoveAll(u => string.Compare(u.UserId, user.UserId, true) == 0 && string.Compare(u.Api, user.Api, true) == 0
                                                        && string.Compare(u.Key, user.Key, true) == 0 && string.Compare(u.ReceiveKey, user.ReceiveKey) == 0);
                    }
                }
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

                switch(routeInfo.Value.HubRoute.BroadcastType)
                {
                    case HubBroadcastType.Group:
                        if (!string.IsNullOrEmpty(route.ReceiveGroup))
                        {
                            await base.Clients.Group(route.ReceiveGroup).SendAsync(route.ReceiveMethod, arg1, arg2);
                        }
                        break;
                    case HubBroadcastType.Individual:
                        if (_connectedUsers.Any())
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
                        break;
                    case HubBroadcastType.All:
                        await base.Clients.All.SendAsync(route.ReceiveMethod, arg1, arg2);
                        break;
                    default: break;
                }
            }            
        }

    }
}
