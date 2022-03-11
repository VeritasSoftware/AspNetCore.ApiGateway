using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Application
{
    internal class EventStoreSubscriptionClientSettings
    {
        public GatewayHubSubscribeEventStoreUser StoreUser { get; set; }
        public EventSourceRouteInfo RouteInfo { get; set; }
        public IEventStoreConnection Connection { get; set; }
        public string GatewayUrl { get; set; }
        public string ConnectionId { get; set; }
        public EventStoreSubscriptionClient Client { get; set; }
        public ILogger<ApiGatewayLog> Logger { get; set; }
    }

    internal class ConnectedEventStoreUser
    {
        public string ConnectionId { get; set; }
        public GatewayHubSubscribeEventStoreUser StoreUser { get; set; }
    }

    internal static class EventStoreClientFactory
    {
        public static List<EventStoreSubscriptionClientSettings> Subscriptions { get; set; } = new List<EventStoreSubscriptionClientSettings>();
        public static List<ConnectedEventStoreUser> ConnectedUsers { get; set; } = new List<ConnectedEventStoreUser>();

        static readonly object _lockObject = new object();

        public static async Task CreateAsync(EventStoreSubscriptionClientSettings subscriptionClientSettings)
        {
            if(!Subscriptions.Any(x => (x.StoreUser.Api == subscriptionClientSettings.StoreUser.Api)
                                            && (x.StoreUser.Key == subscriptionClientSettings.StoreUser.Key)
                                            && (x.RouteInfo.StreamName == subscriptionClientSettings.RouteInfo.StreamName) 
                                            && (x.RouteInfo.GroupName == subscriptionClientSettings.RouteInfo.GroupName)))
            {
                var client = new EventStoreSubscriptionClient(subscriptionClientSettings);

                await client.ConnectAsync();

                subscriptionClientSettings.Client = client;
                
                Subscriptions.Add(subscriptionClientSettings);
            }

            lock(_lockObject)
            {
                if (!ConnectedUsers.Any(x => (x.ConnectionId == subscriptionClientSettings.ConnectionId)
                                             && (x.StoreUser.Api == subscriptionClientSettings.StoreUser.Api)
                                             && (x.StoreUser.Key == subscriptionClientSettings.StoreUser.Key)))
                {
                    ConnectedUsers.Add(new ConnectedEventStoreUser
                    {
                        ConnectionId = subscriptionClientSettings.ConnectionId,
                        StoreUser = subscriptionClientSettings.StoreUser
                    });
                }
            }            
        }
    }

    internal class EventStoreSubscriptionClient : IDisposable
    {
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly EventSourceRouteInfo _routeInfo;
        private readonly string _gatewayHubUrl;
        private readonly GatewayHubSubscribeEventStoreUser _storeUser;
        private HubConnection _hubConnection;
        private readonly ILogger<ApiGatewayLog> _logger;

        private EventStorePersistentSubscriptionBase EventStorePersistentSubscriptionBase { get; set; }

        public EventStoreSubscriptionClient(EventStoreSubscriptionClientSettings settings)
        {
            _eventStoreConnection = settings.Connection;
            _routeInfo = settings.RouteInfo;
            _gatewayHubUrl = settings.GatewayUrl;
            _storeUser = settings.StoreUser;
            _logger = settings.Logger;
        }

        public async Task ConnectAsync()
        {
            _logger?.LogInformation($"Connecting to the Event Store Server.");

            await _eventStoreConnection.ConnectAsync();

            _logger?.LogInformation($"Finished connecting to the Event Store Server.");

            _logger?.LogInformation($"Connecting to Persistent Subscription in the Event Store Server. Stream name: {_routeInfo.StreamName}, Group name: {_routeInfo.GroupName}.");

            EventStorePersistentSubscriptionBase = await _eventStoreConnection.ConnectToPersistentSubscriptionAsync(
                   _routeInfo.StreamName,
                   _routeInfo.GroupName,
                   EventAppeared,
                   SubscriptionDropped,
                   null,
                   10,
                   true
            );

            _logger?.LogInformation($"Finished connecting to Persistent Subscription in the Event Store Server. Stream name: {_routeInfo.StreamName}, Group name: {_routeInfo.GroupName}.");

            _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_gatewayHubUrl)
                    .WithAutomaticReconnect()
                    .AddNewtonsoftJsonProtocol()
                    .Build();

            await _hubConnection.StartAsync();
        }

        private async void SubscriptionDropped(EventStorePersistentSubscriptionBase subscription, SubscriptionDropReason reason, Exception ex)
        {
            await ConnectAsync();
        }

        private async void EventAppeared(EventStorePersistentSubscriptionBase subscription, ResolvedEvent resolvedEvent)
        {
            var strResolvedEvent = JsonConvert.SerializeObject(resolvedEvent, Formatting.Indented);

            await _hubConnection.InvokeAsync("EventStoreEventAppeared", _storeUser, strResolvedEvent);
        }

        public void Dispose()
        {
            try
            {
                EventStorePersistentSubscriptionBase.Stop(TimeSpan.FromSeconds(15));
            }
            catch(Exception)
            {

            }            
        }
    }
}
