### Api Gateway Event Sourcing

The framework supports event sourcing using **[Event Store](https://www.eventstore.com/)**.

Your Api Gateway connects to the back-end Event Store Server.

The client talks to the Api Gateway using web sockets (SignalR).

You can

* publish Events to the downstream Event Store Server stream.

* subscribe to Events from a downstream Event Store Server stream

via the Gateway SignalR Hub.

## Your Gateway API

You set up routes in the ApiOrchestrator to interact with downstream Event Store Server.

You talk to this backend Event Store Server, set up a EventSource and Route in your Gateway API's **Api Orchestrator**.

```C#
orchestrator.AddEventSource("eventsourceservice", BuildEventSourceConnection, "281802b8-6f19-4b9d-820c-9ed29ee127f3")
                    .AddRoute("mystream", new EventSourceRouteInfo { ReceiveMethod = "ReceiveMyStreamEvent", Type = EventSourcingType.EventStore, OperationType = EventSourcingOperationType.PublishSubscribe, StreamName = "my-stream", GroupName = "my-group" });

```

and provide the Url to the Gateway Hub:

```C#
orchestrator.GatewayHubUrl = "https://localhost:44360/GatewayHub";
```

The **RouteKey** (eg. Guid) is to be specified if you want to publish and subscribe.

The **ReceiveMethod** is the method that will receive the events from the downstream Event Store stream.

The **OperationType** specifies the type of operation allowed on the route. Options are PublishOnly, SubscribeOnly, PublishSubscribe. The default is PublishSubscribe.

The **StreamName** is the name of the downstream Event Store stream.

The **GroupName** is the name of the downstream Event Store stream subscription group.

**Note:-** The downstream Event Store stream must have an existing [**persistent subscription**](https://developers.eventstore.com/server/v20.10/persistent-subscriptions.html#persistent-subscription) for that stream & group.

The developers guide to [**creating persistent subscriptions**](https://developers.eventstore.com/clients/dotnet/5.0/subscriptions.html#persistent-subscriptions) on the Event Store Server.

```C#
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
```

In this you set up the **Connection** to the downstream Event Store Server using the **[EventStore.Client](https://www.nuget.org/packages/EventStore.Client/21.2.2)** nuget package library.

### Gateway Hub

There is a **GatewayHub** which the Client can use to publish and subscribe.

To hook up the GatewayHub, in your Gateway API project Startup.cs:

```C#
public void ConfigureServices(IServiceCollection services)
{
    //Hook up GatewayHub using SignalR
    services.AddSignalR().AddNewtonsoftJsonProtocol();          

    //Api gateway
    services.AddApiGateway();

    services.AddControllers();

    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "My Api Gateway", Version = "v1" });
    });            
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    .
    .
    //Api gateway
    app.UseApiGateway(orchestrator => ApiOrchestration.Create(orchestrator, app));

    app.UseEndpoints(endpoints =>
    {
        //GatewayHub endpoint
        endpoints.MapHub<GatewayHub>("/gatewayhub");
        endpoints.MapControllers();
    });
}
```

### Security

If you are going to publish or subscribe on the downstream Event Store Server stream,

you have to specify the **RouteKey**.

## Client

In your **Client**, connect to the GatewayHub and listen to **ReceiveMyStreamEvent**.

You will have to use a SignalR client library to do this.

You can read more on SignalR client libraries for .Net, Java, Javascript etc. [here](https://docs.microsoft.com/en-us/aspnet/core/signalr/client-features?view=aspnetcore-6.0).

You have to subscribe to the route (stream) once, before you start receiving Events.

```C#
var conn = new HubConnectionBuilder()
                .WithUrl("https://localhost:44360/GatewayHub")
                .AddNewtonsoftJsonProtocol()
                .Build();

conn.On("ReceiveMyStreamEvent", new Type[] { typeof(object), typeof(object) }, (arg1, arg2) =>
{
    dynamic parsedJson = JsonConvert.DeserializeObject(arg1[0].ToString());
    var evt = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
    Console.WriteLine(evt);
    return Task.CompletedTask;
}, new object());

await conn.StartAsync();
```

Here, arg1 contains the array of Events sent to all Clients subscribing to the downstream Event Store Server stream.

The Event has the below Json format:

```Javascript
{
  "Event": {
    "EventStreamId": "my-stream",
    "EventId": "20026749-258a-4310-a8bc-07e11f8a2a11",
    "EventNumber": 348,
    "EventType": "MyEvent",
    "Data": "eyJhIjoiMTUifQ==",
    "Metadata": "e30=",
    "IsJson": false,
    "Created": "2022-03-10T22:26:43.075048",
    "CreatedEpoch": 1646951203075
  },
  "Link": null,
  "OriginalPosition": null,
  "OriginalEvent": {
    "EventStreamId": "my-stream",
    "EventId": "20026749-258a-4310-a8bc-07e11f8a2a11",
    "EventNumber": 348,
    "EventType": "MyEvent",
    "Data": "eyJhIjoiMTUifQ==",
    "Metadata": "e30=",
    "IsJson": false,
    "Created": "2022-03-10T22:26:43.075048",
    "CreatedEpoch": 1646951203075
  },
  "IsResolved": false,
  "OriginalStreamId": "my-stream",
  "OriginalEventNumber": 348
}
```

### Publish event

You can publish an event, by calling a method (**PublishToEventStoreStream**) on the Gateway Hub.

```C#
await conn.InvokeAsync("PublishToEventStoreStream", new
{
    Api = "eventsourceservice",
    Key = "mystream",
    RouteKey = "281802b8-6f19-4b9d-820c-9ed29ee127f3",
    Events = new[]
    {
        new {
            EventId = Guid.NewGuid(),
            Type = "MyEvent",
            Data = Encoding.UTF8.GetBytes("{\"a\":\"15\"}"),
            MetaData = Encoding.UTF8.GetBytes("{}"),
            IsJson = false
        }
    }
});
```

### Subscribe to stream

You can also subscribe to a downstream Event Store stream:

```C#
await conn.InvokeAsync("SubscribeToEventStoreStream", new
{
    Api = "eventsourceservice",
    Key = "mystream",
    RouteKey = "281802b8-6f19-4b9d-820c-9ed29ee127f3"
});
```

You have to subscribe to a route (stream) once, before you start receiving Events.

### Unsubscribe from stream

You can unsubscribe from a downstream Event Store stream:

```C#
await conn.InvokeAsync("UnsubscribeFromEventStoreStream", new
{
    Api = "eventsourceservice",
    Key = "mystream",
    RouteKey = "281802b8-6f19-4b9d-820c-9ed29ee127f3"
});
```