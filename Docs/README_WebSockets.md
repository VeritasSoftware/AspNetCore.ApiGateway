### Api Gateway Web Sockets

The Api Gateway supports calling downstream Web Sockets.

You set up a Hub and Route in the **Api Orchestrator**.

```C#
orchestrator.AddHub("chatservice", BuildHubConnection)
                    .AddRoute("room", new HubRouteInfo { InvokeMethod = "SendMessage" });

```

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

The Gateway provides a **POST** endpoint for accepting requests for downstream Hubs.

You can pass the data in the body of this post request.

You can pass a max of 10 objects in the request.

In Swagger, you would call this endpoint as below:

![API Gateway Swagger](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/Docs/WebSockets.PNG)
