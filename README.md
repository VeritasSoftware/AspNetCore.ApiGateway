# AspNetCore.ApiGateway

## Asp Net Core Api Gateway package.

|Packages|Version|Downloads|
|---------------------------|:---:|:---:|
|*AspNetCore.ApiGateway*|[![Nuget Version](https://img.shields.io/nuget/v/AspNetCore.ApiGateway)](https://www.nuget.org/packages/AspNetCore.ApiGateway)|[![Downloads count](https://img.shields.io/nuget/dt/AspNetCore.ApiGateway)](https://www.nuget.org/packages/AspNetCore.ApiGateway)|
|*AspNetCore.ApiGateway.Client*|[![Nuget Version](https://img.shields.io/nuget/v/AspNetCore.ApiGateway.Client)](https://www.nuget.org/packages/AspNetCore.ApiGateway.Client)|[![Downloads count](https://img.shields.io/nuget/dt/AspNetCore.ApiGateway.Client)](https://www.nuget.org/packages/AspNetCore.ApiGateway.Client)|
|*ts-aspnetcore-apigateway-client*|[![NPM Version](https://img.shields.io/npm/v/ts-aspnetcore-apigateway-client)](https://www.npmjs.com/package/ts-aspnetcore-apigateway-client)|[![Downloads count](https://img.shields.io/npm/dy/ts-aspnetcore-apigateway-client)](https://www.npmjs.com/package/ts-aspnetcore-apigateway-client)|

```diff
+ This project has been on-boarded by the .NET Foundation, in the Seed category.
```

Read [more](https://github.com/dotnet-foundation/projects/issues/255).
Social Media: LinkedIn [post](https://www.linkedin.com/feed/update/urn:li:activity:7168255226624372736/).

[![LinkedIn post](/Docs/LinkedIn.png)](https://www.linkedin.com/feed/update/urn:li:activity:7168255226624372736/)

|**More of my open-source projects**|||
|---------------------------|:---:|:---:|
|*Live Health Checks*|Real-Time Api Health Check Monitoring system|[Browse](https://github.com/VeritasSoftware/LiveHealthChecks)|

## Background

The microservices architecture uses an Api Gateway as shown below.

![Architecture](/Docs/Architecture.png)

**The package:**

*	Makes creating an Api Gateway a breeze!!

## Features

*	Swagger
*	Authorization
*   Filters
    *   Action
    *   Exception
    *   Result
*   Load balancing
*   Response caching
*   Web sockets
*   Event sourcing
*   Request aggregation
*   Middleware service
*   Logging
*   Clients available in
    *   .NET
    *   Typescript

## Gateway as a RESTful Microservice Facade

### Your **Gateway API** is a microservice which exposes endpoints that are a **facade** over your backend API endpoints.

*	GET
*   HEAD
*	POST
*	PUT
*   PATCH
*	DELETE

<div>

</div>
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
        public static void Create(IApiOrchestrator orchestrator, IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            var weatherService = serviceProvider.GetService<IWeatherService>();

            var weatherApiClientConfig = weatherService.GetClientConfig();

            orchestrator.StartGatewayHub = true;
            orchestrator.GatewayHubUrl = "https://localhost:44360/GatewayHub";

            orchestrator.AddApi("weatherservice", "http://localhost:63969/")
                                //Get
                                .AddRoute("forecast", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/forecast", ResponseType = typeof(IEnumerable<WeatherForecast>) })
                                //Head
                                .AddRoute("forecasthead", GatewayVerb.HEAD, new RouteInfo { Path = "weatherforecast/forecast" })
                                //Get with params
                                .AddRoute("typewithparams", GatewayVerb.GET, new RouteInfo {  Path = "weatherforecast/types/{index}"})
                                //Get using custom HttpClient
                                .AddRoute("types", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/types", ResponseType = typeof(string[]), HttpClientConfig = weatherApiClientConfig })
                                //Get with param using custom HttpClient
                                .AddRoute("type", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/types/", ResponseType = typeof(WeatherTypeResponse), HttpClientConfig = weatherApiClientConfig })
                                //Get using custom implementation
                                .AddRoute("forecast-custom", GatewayVerb.GET, weatherService.GetForecast)
                                //Post
                                .AddRoute("add", GatewayVerb.POST, new RouteInfo { Path = "weatherforecast/types/add", RequestType = typeof(AddWeatherTypeRequest), ResponseType = typeof(string[])})
                                //Put
                                .AddRoute("update", GatewayVerb.PUT, new RouteInfo { Path = "weatherforecast/types/update", RequestType = typeof(UpdateWeatherTypeRequest), ResponseType = typeof(string[]) })
                                //Patch
                                .AddRoute("patch", GatewayVerb.PATCH, new RouteInfo { Path = "weatherforecast/forecast/patch", ResponseType = typeof(WeatherForecast) })
                                //Delete
                                .AddRoute("remove", GatewayVerb.DELETE, new RouteInfo { Path = "weatherforecast/types/remove/", ResponseType = typeof(string[]) })
                        .AddApi("stockservice", "http://localhost:63967/")
                                .AddRoute("stocks", GatewayVerb.GET, new RouteInfo { Path = "stock", ResponseType = typeof(IEnumerable<StockQuote>) })
                                .AddRoute("stock", GatewayVerb.GET, new RouteInfo { Path = "stock/", ResponseType = typeof(StockQuote) })                                
                        .AddHub("chatservice", ConnectionHelpers.BuildHubConnection, "2f85e3c6-66d2-48a3-8ff7-31a65073558b")
                                .AddRoute("room", new HubRouteInfo { InvokeMethod = "SendMessage", ReceiveMethod = "ReceiveMessage", ReceiveParameterTypes = new Type[] { typeof(string), typeof(string) } })
                        .AddEventSource("eventsourceservice", ConnectionHelpers.BuildEventSourceConnection, "281802b8-6f19-4b9d-820c-9ed29ee127f3")
                                .AddRoute("mystream", new EventSourceRouteInfo { ReceiveMethod = "ReceiveMyStreamEvent", Type = EventSourcingType.EventStore, OperationType = EventSourcingOperationType.PublishSubscribe, StreamName = "my-stream", GroupName = "my-group" });
        }
    }
```

*	Hook up in Startup.cs

```C#
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IWeatherService, WeatherService>();

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
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Api Gateway");
            });

            //Api gateway
            app.UseApiGateway(orchestrator => ApiOrchestration.Create(orchestrator, app));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //GatewayHub endpoint
                endpoints.MapHub<GatewayHub>("/gatewayhub");
                endpoints.MapControllers();
            });
        }
```

The Gateway Swagger appears as shown below:

![API Gateway Swagger](/Docs/ApiGateway.PNG)

To call the **forecast** Route on the **weather service** Api,

you can enter the **Api key** and **Route key** into Swagger as below:

![API Gateway Swagger](/Docs/ApiGatewayCall.PNG)

This will hit the **weatherforecast/forecast** endpoint on the backend Weather API.

### Using appsettings.json

If you want, you can keep the **ApiKey, RouteKey, backend API base urls and Route path**,

in the **appsettings.json**, read it using a Config Service,

and pass it to the Api Orchestrator in the Create method. 

Read [**more**](/Docs/README_ConfigSettings.md).

### Deployment to Prod

As with any Web API, when there is any code change, the API Gateway too is published and deployed using **Blue/Green deployment**.

This is available in **Azure** & **AWS**.

In Azure App Service, you use **deployment slots** etc. 

*   Read [more](https://learn.microsoft.com/en-us/azure/app-service/deploy-best-practices#use-deployment-slots).
*   Read [more](https://learn.microsoft.com/en-us/azure/container-apps/blue-green-deployment?pivots=azure-cli).

Azure Kubernetes Service (AKS) also supports Blue/Green deployment. Read [more](https://learn.microsoft.com/en-us/azure/architecture/guide/aks/blue-green-deployment-for-aks).

In AWS, you use **Elastic Beanstalk**. Read [more](https://docs.aws.amazon.com/elasticbeanstalk/latest/dg/using-features.CNAMESwap.html).

So, there is no down time.

### Verbs usage & Routing

You can check out how the Api Gateway supported Verbs are used & Routing below.

### [Verbs Usage & Routing](Docs/README_VERBS.md)

### Authorization

Just like a Web API's Authorization Filter, the framework provides a **Gateway Authorization Filter**.

Here you can perform any kind of Authorization you like. There is no prescribed Authorization.

### [Authorization](Docs/README_Authorization.md)

### Customizations

*   Create a new or customize the default **HttpClient** used by all the routes, to hit the backend Api.
*   Create a new or customize the default **HttpClient** which each route uses to hit the backend Api.
*	Use your own custom implementation to hit the backend Api.

For **Request aggregation**, see this section.

### [Customizations](Docs/README_Customizations.md)

### Load Balancing

### [Load Balancing](Docs/README_LoadBalancing.md)

### Response Caching

### [Response Caching](Docs/README_ResponseCaching.md)

### Web Sockets

### [Web Sockets](Docs/README_WebSockets.md)

### Event Sourcing

### [Event Sourcing](Docs/README_EventSourcing.md)

### Filters

   * #### [Action Filters](Docs/README_ActionFilters.md)
   * #### [Exception Filters](Docs/README_ExceptionFilters.md)
   * #### [Result Filters](Docs/README_ResultFilters.md)
    
### Middleware Service

### [Middleware Service](Docs/README_Middleware_Service.md)

### Viewing your Gateway's Api Orchestration

Your Gateway's Api Orchestration is published by **GET /api/Gateway/orchestration** endpoint.

### [Viewing Api Orchestration](Docs/README_Orchestration.md)

### Logging

The Api Gateway uses **ILogger\<ApiGatewayLog>** to create logs. 

In your Gateway API project, this can be used to tap into these logs.

## Clients

The Api Gateway supports a fixed set of endpoints.

All routes go through these endpoints.

The Client application has to talk to these endpoints of the Api Gateway.

A Client library is provided for:

* [**.Net**](Docs/README_Net_Client.md)

* [**Typescript**](Docs/README_Typescript_Client.md)