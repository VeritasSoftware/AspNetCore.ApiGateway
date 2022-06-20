## Api Gateway Client (.Net)

|Packages|Version & Downloads|
|---------------------------|:---:|
|*AspNetCore.ApiGateway.Client*|[![NuGet Version and Downloads count](https://buildstats.info/nuget/AspNetCore.ApiGateway.Client)](https://www.nuget.org/packages/AspNetCore.ApiGateway.Client)|

The .Net client application can talk to the Api Gateway using the Client.

![.Net Client](/Docs/NetClient.png)

Wire up the Client for **dependency injection** in your app, using an extension.

```C#
services.AddApiGatewayClient(settings => settings.ApiGatewayBaseUrl = "http://localhost");
```

Then, you can inject **IApiGatewayClient** in your app.

You can go through these **Integration Tests** to see how the Client is used.

[.Net Client Integraton Tests](/AspNetCore.ApiGateway.Tests/GatewayClientTests.cs)