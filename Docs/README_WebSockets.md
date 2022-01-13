### Api Gateway Web Sockets

The framework uses SignalR as the Web Sockets technology.

Out of the box, there is a SignalR Gateway Hub.

You can set up routes in the ApiOrchestrator to interact with downstream SignalR Hubs.

You can

* send a notification to the downstream Hub.

* receive a notification from the downstream Hub via the Gateway Hub.

## Backend/Downstream Hub

Let us say, you have a downstream ChatHub:

```C#
public class ChatHub : Hub
{
    public async Task SendMessage(Request request)
    {
        await base.Clients.All.SendAsync("ReceiveMessage", request.Name, request.Message);
    }
}
```

## Your Gateway API

You talk to this Hub, set up a Hub and Route in your Gateway API's **Api Orchestrator**.

```C#
orchestrator.AddHub("chatservice", BuildHubConnection, "2f85e3c6-66d2-48a3-8ff7-31a65073558b")
                    .AddRoute("room", new HubRouteInfo { InvokeMethod = "SendMessage", ReceiveMethod = "ReceiveMessage", ReceiveParameterTypes = new Type[] { typeof(string), typeof(string) } });

```

The **ReceiveKey** (eg. Guid) is to be specified if you want to receive notifications from the downstream hub.

```C#
private static HubConnection BuildHubConnection(HubConnectionBuilder builder)
{
    return builder.WithUrl("http://localhost:53353/ChatHub")
                  .AddNewtonsoftJsonProtocol()
                  .Build();
}
```

In this you set up the **Connection** to the downstream Web Sockets using the **Asp Net Core SignalR Client**.

The **InvokeMethod** is the method that is called in the downstream Hub.

The **ReceiveMethod** is the method that will receive the notification from the Hub.

You can set how the notification from the downstream hub is sent from the Gateway hub using **BroadcastType**.

Broadcast Type | Explanation
-- | --
All | The notification from the downstream hub will be sent to all connected Clients.
Group | The notification from the downstream hub will be sent to all connected Clients, who have subscribed to a Group. You must specify the **ReceiveGroup** ie group name.
Individual | The notification from the downstream hub will be sent to individual/specific Clients, who have subscribed to the Route.

The default route BroadcastType is **All**.

The Gateway provides 

* a Hub method (**InvokeDownstreamHub**) for accepting requests for downstream Hubs.
* a **POST** endpoint for accepting requests for downstream Hubs.

### POST endpoint

You can pass the data in the body of this post request.

You can pass a max of 10 objects in the request.

In Swagger, you would call this endpoint as below:

![API Gateway Swagger](/Docs/WebSockets.PNG)

### Gateway Hub

There is a **GatewayHub** which the Client can subscribe to, to get messages from and to send messages to, the back end/downstream Hub.

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
orchestrator.StartGatewayHub = true;
orchestrator.GatewayHubUrl = "https://localhost:44360/GatewayHub";
```

**Note**

You can turn off the GatewayHub by

```C#
orchestrator.StartGatewayHub = false;
```

#### Filter

You can filter the calls to the Gateway Hub.

By implementing **IGatewayHubFilter**.

In your Gateway API project,

*	Create a service like below

```C#
    public class GatewayHubFilterService : IGatewayHubFilter
    {
        public ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext)
        {
            //Do your work here eg.
            //invocationContext.Context.Abort();
            return new ValueTask<object>();
        }

        public Task OnConnectedAsync(HubLifetimeContext context)
        {
            //Do your work here
            return Task.CompletedTask;
        }

        public Task OnDisconnectedAsync(HubLifetimeContext context, Exception exception)
        {
            //Do your work here
            return Task.CompletedTask;
        }
    }
```

*	Wire it up for dependency injection in Startup.cs

```C#
services.AddScoped<IGatewayHubFilter, GatewayHubFilterService>();
.
.
services.AddApiGateway();
```

### Security

You can secure the POST endpoint by implementing interface **IHubPostGatewayAuthorization**.

Please see section [Authorization](/Docs/README_Authorization.md) to learn how to do this.

Also, if you are going to receive notifications from or invoke methods on the downstream Hub,

you have to specify the **ReceiveKey**.

Also, you can turn off the Gateway Hub.

```C#
orchestrator.StartGatewayHub = false;
```

## Client

In your **Client**, connect to the GatewayHub and listen to **ReceiveMessage**.

```C#
var conn = new HubConnectionBuilder()
                .WithUrl("https://localhost:44360/GatewayHub")
                .AddNewtonsoftJsonProtocol()
                .Build();

conn.On("ReceiveMessage", new Type[] { typeof(object), typeof(object) }, (arg1, arg2) =>
{
    return WriteToConsole(arg1);
}, new object());

await conn.StartAsync();
```

Here, arg1 contains the array of objects sent to all Clients subscribing to the downstream Chat Hub SendMessage method.

### Broadcast Types

#### Group

If you have set the route BroadcastType to Group (as shown below), you have to specify a **ReceiveGroup** too.

Then, the user has to subscribe to the Group.

```C#
.AddRoute("room", new HubRouteInfo { BroadcastType = HubBroadcastType.Group, ReceiveGroup = "ChatGroup", InvokeMethod = "SendMessage", ReceiveMethod = "ReceiveMessage", ReceiveParameterTypes = new Type[] { typeof(string), typeof(string) } });
```

Only users who have subscribed, will be sent notifications.

```C#
await conn.InvokeAsync("SubscribeToGroup", new
{
    Api = "chatservice",
    Key = "room",
    ReceiveKey = "2f85e3c6-66d2-48a3-8ff7-31a65073558b",
    ReceiveGroup = "ChatGroup"
});
```

You can invoke **UnsubscribeFromGroup** (with the same param), to stop receiving notifications.

#### Individual

If you have set the route BroadcastType to Individual (as shown below), you have to subscribe to the Route.

```C#
.AddRoute("room", new HubRouteInfo { BroadcastType = HubBroadcastType.Individual, InvokeMethod = "SendMessage", ReceiveMethod = "ReceiveMessage", ReceiveParameterTypes = new Type[] { typeof(string), typeof(string) } })
```

Only users who have subscribed, will be sent notifications.

```C#
await conn.InvokeAsync("SubscribeToRoute", new
{
    Api = "chatservice",
    Key = "room",                
    ReceiveKey = "2f85e3c6-66d2-48a3-8ff7-31a65073558b",
    UserId = "JohnD"
});
```

You can invoke **UnsubscribeFromRoute** (with the same param), to stop receiving notifications.

### Invoking downstream hub

You can invoke a method on a downstream hub, by calling a method (**InvokeDownstreamHub**) on the Gateway Hub.

```C#
await conn.InvokeAsync("InvokeDownstreamHub", new
{
  Api = "chatservice",
  Key = "room",
  ReceiveKey = "2f85e3c6-66d2-48a3-8ff7-31a65073558b",
  Data = new
  {
     Name = "John",
     Message = "Hello!"
  }
});
```

You can also send an array of objects like below:

```C#
await conn.InvokeAsync("InvokeDownstreamHub", new
{
  Api = "chatservice",
  Key = "room",
  ReceiveKey = "2f85e3c6-66d2-48a3-8ff7-31a65073558b",
  DataArray = new[]
  {
    new {
      Name = "John",
      Message = "Hello!"
    }
  }
});
```
