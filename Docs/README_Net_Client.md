## Api Gateway Client (.Net)

The .Net client application can talk to the Api Gateway using the Client.

![.Net Client](/Docs/NetClient.png)

There is a **dependency injection** extension to wire up the Client in your app.

```C#
services.AddApiGatewayClient(settings => settings.ApiGatewayUrl = "http://localhost/api/Gateway");
```

You can go through these **Integration Tests** to see how the Client is used.

[.Net Client Integraton Tests](/AspNetCore.ApiGateway.Tests/GatewayClientTests.cs)