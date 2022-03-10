### Api Gateway Event Sourcing

The framework supports event sourcing using **[Event Store](https://www.eventstore.com/)**.

You can set up routes in the ApiOrchestrator to interact with downstream Event Store Server.

You can

* publish an event to the downstream Event Store Server stream.

* subscribe to events from a downstream Event Store Server stream

via the Gateway Hub.

## Your Gateway API

You talk to this backend Event Store Server, set up a EventSource and Route in your Gateway API's **Api Orchestrator**.

```C#
orchestrator.AddEventSource("eventsourceservice", BuildEventSourceConnection, "281802b8-6f19-4b9d-820c-9ed29ee127f3")
                    .AddRoute("mystream", new EventSourceRouteInfo { ReceiveMethod = "ReceiveMyStreamEvent", Type = EventSourcingType.EventStoreDb, StreamName = "my-stream", GroupName = "my-group" });

```

The **ReceiveKey** (eg. Guid) is to be specified if you want to publish and subscribe.

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

The **ReceiveMethod** is the method that will receive the events from the downstream Event Store stream.

The **StreamName** is the name of the downstream Event Store stream.

The **GroupName** is the name of the downstream Event Store stream subscription group.

The **UserId** and **Password** are the credentials on the downstream Event Store Server.

**Note:-** The downstream Event Store stream must have a persistent subscription created for that stream & group.

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

In your Api Orchestration, provide the Url to the Gateway Hub:

```C#
orchestrator.GatewayHubUrl = "https://localhost:44360/GatewayHub";
```

### Security

If you are going to publish or subscribe on the downstream Event Store Server stream,

you have to specify the **ReceiveKey**.

```C#
orchestrator.StartGatewayHub = false;
```

## Client

In your **Client**, connect to the GatewayHub and listen to **ReceiveMyStreamEvent**.

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

Here, arg1 contains the array of events sent to all Clients subscribing to the downstream Event Store Server stream.

### Publish event

You can publish an event, by calling a method (**PublishToEventStoreStream**) on the Gateway Hub.

```C#
await conn.InvokeAsync("PublishToEventStoreStream", new
{
    Api = "eventsourceservice",
    Key = "mystream",
    ReceiveKey = "281802b8-6f19-4b9d-820c-9ed29ee127f3",
    Events = new[]
    {
        new {
            EventId = Guid.NewGuid(),
            Type = "MyEvent",
            Data = Encoding.UTF8.GetBytes("{\"a\":\"15\"}"),
            MetaData = Encoding.UTF8.GetBytes("{}")
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
    ReceiveKey = "281802b8-6f19-4b9d-820c-9ed29ee127f3"
});
```