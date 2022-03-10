using AspNetCore.ApiGateway;
using AspNetCore.ApiGateway.Application;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ApiGateway.API
{
    public static class ApiOrchestration
    {
        public static void Create(IApiOrchestrator orchestrator, IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            var weatherService = serviceProvider.GetService<IWeatherService>();

            var weatherApiClientConfig = weatherService.GetClientConfig();

            orchestrator.StartGatewayHub = false;
            orchestrator.GatewayHubUrl = "https://localhost:44360/GatewayHub";

            orchestrator.AddApi("weatherservice", "http://localhost:63969/")
                                //Get
                                .AddRoute("forecast", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/forecast", ResponseType = typeof(IEnumerable<WeatherForecast>) })
                                //Head
                                .AddRoute("forecasthead", GatewayVerb.HEAD, new RouteInfo { Path = "weatherforecast/forecast" })
                                //Get using custom HttpClient
                                .AddRoute("types", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/types", ResponseType = typeof(string[]), HttpClientConfig = weatherApiClientConfig })
                                //Get with param using custom HttpClient
                                .AddRoute("type", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/types/", ResponseType = typeof(WeatherTypeResponse), HttpClientConfig = weatherApiClientConfig })
                                //Get using custom implementation
                                .AddRoute("typescustom", GatewayVerb.GET, weatherService.GetTypes)
                                //Post
                                .AddRoute("add", GatewayVerb.POST, new RouteInfo { Path = "weatherforecast/types/add", RequestType = typeof(AddWeatherTypeRequest), ResponseType = typeof(string[]) })
                                //Put
                                .AddRoute("update", GatewayVerb.PUT, new RouteInfo { Path = "weatherforecast/types/update", RequestType = typeof(UpdateWeatherTypeRequest), ResponseType = typeof(string[]) })
                                //Patch
                                .AddRoute("patch", GatewayVerb.PATCH, new RouteInfo { Path = "weatherforecast/forecast/patch", ResponseType = typeof(WeatherForecast) })
                                //Delete
                                .AddRoute("remove", GatewayVerb.DELETE, new RouteInfo { Path = "weatherforecast/types/remove/", ResponseType = typeof(string[]) })
                        .AddApi("stockservice", "http://localhost:63967/")
                                .AddRoute("stocks", GatewayVerb.GET, new RouteInfo { Path = "stock", ResponseType = typeof(IEnumerable<StockQuote>) })
                                .AddRoute("stock", GatewayVerb.GET, new RouteInfo { Path = "stock/", ResponseType = typeof(StockQuote) })
                        .AddHub("chatservice", BuildHubConnection, "2f85e3c6-66d2-48a3-8ff7-31a65073558b")
                                .AddRoute("room", new HubRouteInfo { InvokeMethod = "SendMessage", ReceiveMethod = "ReceiveMessage", BroadcastType = HubBroadcastType.Group, ReceiveGroup = "ChatGroup", ReceiveParameterTypes = new Type[] { typeof(string), typeof(string) } })
                        .AddEventSource("eventsourceservice", BuildEventSourceConnection, "281802b8-6f19-4b9d-820c-9ed29ee127f3")
                                .AddRoute("mystream", new EventSourceRouteInfo { ReceiveMethod = "ReceiveMyStreamEvent", Type = EventSourcingType.EventStoreDb, StreamName = "my-test-stream", GroupName = "my-group" });
        }

        private static HubConnection BuildHubConnection(HubConnectionBuilder builder)
        {
            return builder.WithUrl("https://localhost:44339/chathub")
                          .AddNewtonsoftJsonProtocol()
                          .Build();
        }

        private static object BuildEventSourceConnection()
        {
            var address = IPAddress.Parse("127.0.0.1");
            var tcpPort = 1113;

            var userName = "admin";
            var password = "********";

            var _connectionSettings = ConnectionSettings.Create();
            _connectionSettings.EnableVerboseLogging()
                .UseDebugLogger()
                .UseConsoleLogger()
                .KeepReconnecting()
                .DisableServerCertificateValidation()
                .DisableTls()
                .LimitAttemptsForOperationTo(3)
                .LimitRetriesForOperationTo(3)
                .SetHeartbeatTimeout(TimeSpan.FromSeconds(3600))
                .SetHeartbeatInterval(TimeSpan.FromSeconds(3600))
                .WithConnectionTimeoutOf(TimeSpan.FromSeconds(3600))
                .Build();

            var connection = EventStoreConnection.Create(
                $"ConnectTo=tcp://{address}:{tcpPort};DefaultUserCredentials={userName}:{password};",
                _connectionSettings);

            return connection;
        }
    }
}
