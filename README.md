# AspNetCore.ApiGateway

## Asp Net Core Api Gateway package.

[![Build Status](https://travis-ci.com/VeritasSoftware/AspNetCore.ApiGateway.svg?branch=master)](https://travis-ci.com/VeritasSoftware/AspNetCore.ApiGateway)

The microservices architecture uses an Api Gateway as shown below.

![Architecture](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/Architecture.png)

**The package:**

*	Makes creating an Api Gateway a breeze!!
*	Swagger support.

In the solution, there are 2 back end Apis : **Weather API** and **Stock API**.

## In your Gateway API project

**Add a reference to the package and...**

*	Create an **Api Orchestration** as shown below.

```C#
    public static class ApiOrchestration
    {
        public static void Create(IApiOrchestrator orchestrator, IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            var weatherService = serviceProvider.GetService<IWeatherService>();

            var weatherApiClientConfig = weatherService.GetClientConfig();

            orchestrator.AddApi("weatherservice", "http://localhost:58262/")
                                .AddRoute("forecast", new RouteInfo { Path = "weatherforecast/forecast", ResponseType = typeof(IEnumerable<WeatherForecast>) })
                                .AddRoute("types", new RouteInfo { Path = "weatherforecast/types", ResponseType = typeof(string[]), HttpClientConfig = weatherApiClientConfig })
                                .AddRoute("type", new RouteInfo { Path = "weatherforecast/types/", ResponseType = typeof(WeatherTypeResponse), HttpClientConfig = weatherApiClientConfig })
                                .AddRoute("typescustom", weatherService.GetTypes)
                        .ToOrchestrator()
                        .AddApi("stockservice", "http://localhost:58352/")
                                .AddRoute("stocks", new RouteInfo { Path = "stock", ResponseType = typeof(IEnumerable<StockQuote>) })
                                .AddRoute("stock", new RouteInfo { Path = "stock/", ResponseType = typeof(StockQuote) });
        }
    }
```

*	In Startup.cs

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

To call the forecast route on the weather service,

you can enter the Api key and Route key into Swagger as below:

![API Gateway Swagger](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/ApiGatewayCall.PNG)

This will hit the **weatherforecast/forecast** endpoint on the backend Weather API.

### Features

*	You can use your own **HttpClient** to hit the backend Api.
*	You can create your own implementation to hit the backend Api.