# API Gateway as Azure Functions Facade

The API Gateway is engineered as a Azure Functions facade.

You can hook up **Authorization** etc. just like any Azure Function.

|Packages|Version|Downloads|
|---------------------------|:---:|:---:|
|*Veritas.AspNetCore.ApiGateway.Minimal*|[![Nuget Version](https://img.shields.io/nuget/v/Veritas.AspNetCore.ApiGateway.Minimal)](https://www.nuget.org/packages/Veritas.AspNetCore.ApiGateway.Minimal)|[![Downloads count](https://img.shields.io/nuget/dt/Veritas.AspNetCore.ApiGateway.Minimal)](https://www.nuget.org/packages/Veritas.AspNetCore.ApiGateway.Minimal)|
|*AspNetCore.ApiGateway.Client*|[![Nuget Version](https://img.shields.io/nuget/v/AspNetCore.ApiGateway.Client)](https://www.nuget.org/packages/AspNetCore.ApiGateway.Client)|[![Downloads count](https://img.shields.io/nuget/dt/AspNetCore.ApiGateway.Client)](https://www.nuget.org/packages/AspNetCore.ApiGateway.Client)|
|*ts-aspnetcore-apigateway-client*|[![NPM Version](https://img.shields.io/npm/v/ts-aspnetcore-apigateway-client)](https://www.npmjs.com/package/ts-aspnetcore-apigateway-client)|[![Downloads count](https://img.shields.io/npm/dy/ts-aspnetcore-apigateway-client)](https://www.npmjs.com/package/ts-aspnetcore-apigateway-client)|

## Features

*	Authorization
*   Load balancing
*   Response caching
*   Request aggregation
*   Logging
*   Clients available in
    *   .NET
    *   Typescript

### Your **Gateway API** is a minimal API host which exposes endpoints that are a **facade** over your backend API endpoints.

*   HEAD
*	GET
*	POST
*	PUT
*   PATCH
*	DELETE

<img src="/Docs/FacadeDesignPattern.PNG" style="width:60%;height:auto;max-width:500px;" alt="API Gateway Facade" >

## Implementation

In the solution, there are 2 **back end APIs** : **Weather API** and **Stock API**.

For eg. To make a GET call to the backend API, you would set up an Api and a GET Route in your Gateway API's **Api Orchestrator**.

Then, the client app would make a GET call to the Gateway API which would make a GET call to the backend API using HttpClient.

## In your Backend API

Let us say you have a GET endpoint like this.

*	**HTTP GET - /weatherforecast/forecast**

## In your Gateway API

You add a Route for the backend GET call in the **Api Orchrestrator**.

You create a backend API with ApiKey called **weatherservice** for eg.
And, a Route with RouteKey called **forecast** for eg.

So, the call to the Gateway would become:

*	**HTTP GET - /weatherservice/forecast**

**Add a reference to the package and...**

*	Create an **Api Orchestration**.
	
	You create an Api (weatherservice) and add a Route (forecast).

```C#
public static class ApiOrchestration
{
    public static void Create(IApiOrchestrator orchestrator, IHost app)
    {
        var serviceProvider = app.Services;

        var weatherService = serviceProvider.GetRequiredService<IWeatherService>();

        var weatherApiClientConfig = weatherService.GetClientConfig();

        orchestrator.AddApi("weatherservice", "https://localhost:5003/")
                            //Get
                            .AddRoute("forecast", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/forecast", ResponseType = typeof(IEnumerable<WeatherForecast>) })
                            //Head
                            .AddRoute("forecasthead", GatewayVerb.HEAD, new RouteInfo { Path = "weatherforecast/forecast" })
                            //Get with params
                            .AddRoute("typewithparams", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/types/{index}" })
                            //Get using custom HttpClient
                            .AddRoute("types", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/types", ResponseType = typeof(string[]), HttpClientConfig = weatherApiClientConfig })
                            //Get with param using custom HttpClient
                            .AddRoute("type", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/types/", ResponseType = typeof(WeatherTypeResponse), HttpClientConfig = weatherApiClientConfig })
                            //Get using custom implementation
                            .AddRoute("forecast-custom", GatewayVerb.GET, weatherService.GetForecast)
                            //Post
                            .AddRoute("add", GatewayVerb.POST, new RouteInfo { Path = "weatherforecast/types/add", RequestType = typeof(AddWeatherTypeRequest), ResponseType = typeof(string[]) })
                            //Put
                            .AddRoute("update", GatewayVerb.PUT, new RouteInfo { Path = "weatherforecast/types/update", RequestType = typeof(UpdateWeatherTypeRequest), ResponseType = typeof(string[]) })
                            //Patch
                            .AddRoute("patch", GatewayVerb.PATCH, new RouteInfo { Path = "weatherforecast/forecast/patch", ResponseType = typeof(WeatherForecast) })
                            //Delete
                            .AddRoute("remove", GatewayVerb.DELETE, new RouteInfo { Path = "weatherforecast/types/remove/", ResponseType = typeof(string[]) })
                    .AddApi("stockservice", "https://localhost:5005/")
                            .AddRoute("stocks", GatewayVerb.GET, new RouteInfo { Path = "stock", ResponseType = typeof(IEnumerable<StockQuote>) })
                            .AddRoute("stock", GatewayVerb.GET, new RouteInfo { Path = "stock/", ResponseType = typeof(StockQuote) });
    }
}
```

*	Hook up in Startup.cs

```C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddTransient<IWeatherService, WeatherService>();

    //services.AddSingleton<IApiGatewayConfigService, ApiGatewayConfigService>(); 

    //Api gateway
    services.AddApiGateway(options =>
    {
        options.UseResponseCaching = false;
        options.ResponseCacheSettings = new ApiGatewayResponseCacheSettings
        {
            Duration = 60, //default for all routes
            Location = ResponseCacheLocation.Any,
            //Use VaryByQueryKeys to vary the response for each apiKey & routeKey
            VaryByQueryKeys = new[] { "apiKey", "routeKey" }
        };
    });                   
}

public void Configure(IHost app)
{
    //Api gateway
    app.UseApiGateway(orchestrator => ApiOrchestration.Create(orchestrator, app));
}
```

The Functions start as shown below:

![API Gateway Azure Functions](/Docs/APIGatewayAzureFunctions.png)

To call the **forecast** Route on the **weather service** Api,

you can enter the **Api key** and **Route key** into Swagger as below:

![API Gateway Minimal Swagger](/Docs/ApiGatewayAzureFunctionCall.png)

This will hit the **weatherforecast/forecast** endpoint on the backend Weather API.

### Using appsettings.json

If you want, you can keep the **ApiKey, RouteKey, backend API base urls and Route path**,

in the **appsettings.json**, read it using a Config Service,

and pass it to the Api Orchestrator in the Create method. 

Read [**more**](/Docs/README_ConfigSettings.md).

### Verbs usage & Routing

You can check out how the Api Gateway supported Verbs are used & Routing below.

### [Verbs Usage & Routing](Docs/README_VERBS.md)

### Customizations

*   Create a new or customize the default **HttpClient** used by all the routes, to hit the backend Api.
*   Create a new or customize the default **HttpClient** which each route uses to hit the backend Api.
*	Use your own custom implementation to hit the backend Api.

For **Request aggregation**, see this section.

### [Customizations](Docs/README_Customizations.md)

### Load Balancing

### [Load Balancing](Docs/README_LoadBalancing.md)

## Clients

The Api Gateway supports a fixed set of endpoints.

All routes go through these endpoints.

The Client application has to talk to these endpoints of the Api Gateway.

A Client library is provided for:

* [**.Net**](Docs/README_Net_Client.md)

* [**Typescript**](Docs/README_Typescript_Client.md)

## Making requests to Azure Functions Gateway

These [**Tests**](/AspNetCore.ApiGateway.Tests/AzureFunctionsGatewayTests.cs) show how to make calls to the Azure Functions Gateway.