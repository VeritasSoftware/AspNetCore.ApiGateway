# API Gateway as Minimal API Facade

The API Gateway is engineered as a Minimal API facade.

You can hook up **Authorization**, **Swagger** etc. just like any Minimal API.

## Features

*	Swagger
*	Authorization
*   Load balancing
*   Response caching
*   Request aggregation
*   Middleware service
*   Logging
*   Clients available in
    *   .NET
    *   Typescript

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
        //Hook up GatewayHub using SignalR
        services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
        {
            builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin();
        }));

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

        services.AddEndpointsApiExplorer(); // Required for Minimal APIs
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "My Minimal API Gateway",
                Description = "A simple example of ASP.NET Core Minimal API with Swagger",
                Version = "v1"
            });
        });

        services.AddMvc();            
    }

    //public void Configure(IApplicationBuilder app, WebApplication webApplication)
    public void Configure(IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Minimal API Gateway V1");
            // Optional: set the UI to load at the app root URL
            // c.RoutePrefix = string.Empty; 
        });

        //webApplication.
        app.UseCors("CorsPolicy");

        //Api gateway
        app.UseApiGateway(orchestrator => ApiOrchestration.Create(orchestrator, app));

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            // Api Gateway Minimal API
            endpoints.MapApiGatewayHead();
            endpoints.MapApiGatewayGet();
            endpoints.MapApiGatewayPost();
            endpoints.MapApiGatewayPut();
            endpoints.MapApiGatewayDelete();
            endpoints.MapApiGatewayPatch();
            endpoints.MapApiGatewayGetOrchestration();
        });
    }
```

The Gateway Swagger appears as shown below:

![API Gateway Minimal Swagger](/Docs/APIGatewayMinimal.png)

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

### Middleware Service

### [Middleware Service](Docs/README_Middleware_Service.md)

## Clients

The Api Gateway supports a fixed set of endpoints.

All routes go through these endpoints.

The Client application has to talk to these endpoints of the Api Gateway.

A Client library is provided for:

* [**.Net**](Docs/README_Net_Client.md)

* [**Typescript**](Docs/README_Typescript_Client.md)

## Making requests to Minimal API Gateway

These [**Tests**](/AspNetCore.ApiGateway.Tests/MinimalAPIGatewayTests.cs) show how to make calls to the Minimal API Gateway.