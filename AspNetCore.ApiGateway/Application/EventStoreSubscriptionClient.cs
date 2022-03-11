using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
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
    }

    internal static class EventStoreClientFactory
    {
        public static List<EventStoreSubscriptionClientSettings> Subscriptions { get; set; } = new List<EventStoreSubscriptionClientSettings>();

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
            }

            if (!Subscriptions.Any(x => (x.ConnectionId == subscriptionClientSettings.ConnectionId)
                                             && (x.StoreUser.Api == subscriptionClientSettings.StoreUser.Api)
                                             && (x.StoreUser.Key == subscriptionClientSettings.StoreUser.Key)
                                             && (x.RouteInfo.StreamName == subscriptionClientSettings.RouteInfo.StreamName)
                                             && (x.RouteInfo.GroupName == subscriptionClientSettings.RouteInfo.GroupName)))
            {
                Subscriptions.Add(subscriptionClientSettings);
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

        private EventStorePersistentSubscriptionBase EventStorePersistentSubscriptionBase { get; set; }

        public EventStoreSubscriptionClient(EventStoreSubscriptionClientSettings settings)
        {
            _eventStoreConnection = settings.Connection;
            _routeInfo = settings.RouteInfo;
            _gatewayHubUrl = settings.GatewayUrl;
            _storeUser = settings.StoreUser;
            _eventStoreConnection.ConnectAsync().ConfigureAwait(true);
        }

        public async Task ConnectAsync()
        {            
            EventStorePersistentSubscriptionBase = await _eventStoreConnection.ConnectToPersistentSubscriptionAsync(
                   _routeInfo.StreamName,
                   _routeInfo.GroupName,
                   EventAppeared,
                   SubscriptionDropped,
                   null,
                   10,
                   true
            );

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

        private void EventAppeared(EventStorePersistentSubscriptionBase subscription, ResolvedEvent resolvedEvent)
        {
            var strResolvedEvent = JsonConvert.SerializeObject(resolvedEvent);

            _hubConnection.InvokeAsync("EventStoreEventAppeared", _storeUser, strResolvedEvent);
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
