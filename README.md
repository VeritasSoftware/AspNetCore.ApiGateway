# AspNetCore.ApiGateway

## Asp Net Core Api Gateway package.

[![Build Status](https://travis-ci.com/VeritasSoftware/AspNetCore.ApiGateway.svg?branch=master)](https://travis-ci.com/VeritasSoftware/AspNetCore.ApiGateway)

The microservices architecture uses an Api Gateway as shown below.

![Architecture](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/Architecture.png)

**The package:**

*	Makes creating an Api Gateway a breeze!!
*	Authorization support
*	Swagger support.

In the solution, there are 2 **back end APIs** : **Weather API** and **Stock API**.

### Your **Gateway API** exposes endpoints which are a **facade** over your backend API endpoints.

*	GET
*	POST
*	PUT
*	DELETE

For eg. To make a GET call to the backend API, you would set up an Api and a Route in your Gateway API's **Api Orchestrator**.

Then, the client app would make a GET call to the Gateway API which would make a GET call to the backend API using HttpClient.

## In your Backend API project

Let us say you have a GET endpoint like this.

*	**HTTP GET - /weatherforecast/forecast**

## In your Gateway API project

You add a Route for the backend GET call in the **Api Orchrestrator**.

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

            orchestrator.AddApi("weatherservice", "http://localhost:63969/")
                                //Get
                                .AddRoute("forecast", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/forecast", ResponseType = typeof(IEnumerable<WeatherForecast>) })
                                //Get using custom HttpClient
                                .AddRoute("types", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/types", ResponseType = typeof(string[]), HttpClientConfig = weatherApiClientConfig })
                                //Get with param using custom HttpClient
                                .AddRoute("type", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/types/", ResponseType = typeof(WeatherTypeResponse), HttpClientConfig = weatherApiClientConfig })
                                //Get using custom implementation
                                .AddRoute("typescustom", GatewayVerb.GET, weatherService.GetTypes)
                                //Post
                                .AddRoute("add", GatewayVerb.POST, new RouteInfo { Path = "weatherforecast/types/add", RequestType = typeof(AddWeatherTypeRequest), ResponseType = typeof(string[])})
                                //Put
                                .AddRoute("update", GatewayVerb.PUT, new RouteInfo { Path = "weatherforecast/types/update", RequestType = typeof(UpdateWeatherTypeRequest), ResponseType = typeof(string[]) })
                                //Delete
                                .AddRoute("remove", GatewayVerb.DELETE, new RouteInfo { Path = "weatherforecast/types/remove/", ResponseType = typeof(string[]) })
                        .ToOrchestrator()
                        .AddApi("stockservice", "http://localhost:63967/")
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
                endpoints.MapControllers();
            });
        }
```

The Gateway Swagger appears as shown below:

![API Gateway Swagger](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/ApiGateway.PNG)

To call the **forecast** Route on the **weather service** Api,

you can enter the Api key and Route key into Swagger as below:

![API Gateway Swagger](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/ApiGatewayCall.PNG)

This will hit the **weatherforecast/forecast** endpoint on the backend Weather API.

You can check out how the Api Gateway supported Verbs are used below.

### [Verbs Usage](README_VERBS.md)

You can check out how the Api Gateway's endpoint Authorization support below.

### [Authorization](README_Authorization.md)

### Features

*	You can use your own **HttpClient** to hit the backend Api.
*	You can create your own implementation to hit the backend Api.