## Api Gateway Client (.Net)

|Packages|Version & Downloads|
|---------------------------|:---:|
|*AspNetCore.ApiGateway.Client*|[![Nuget Version](https://img.shields.io/nuget/v/AspNetCore.ApiGateway.Client)](https://www.nuget.org/packages/AspNetCore.ApiGateway.Client)|[![Downloads count](https://img.shields.io/nuget/dt/AspNetCore.ApiGateway.Client)](https://www.nuget.org/packages/AspNetCore.ApiGateway.Client)|

The .Net client application can talk to the Api Gateway using the Client.

Wire up the Client for **dependency injection** in your app, using an extension.

```C#
services.AddApiGatewayClient(settings => settings.ApiGatewayBaseUrl = "http://localhost");
```

Then, you can inject **IApiGatewayClient** in your app.

You can go through these **Integration Tests** to see how the Client is used.

[.Net Client - Microservice version - Integraton Tests](/AspNetCore.ApiGateway.Tests/GatewayClientTests.cs)

[.Net Client - Minimal API version - Integraton Tests](/AspNetCore.ApiGateway.Tests/MinimalGatewayClientTests.cs)