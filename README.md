# AspNetCore.ApiGateway

## Asp Net Core Api Gateway package.

**The package:**

*	Makes creating an Api Gateway a breeze!!

**Add a reference to the package and create an Api Orchestration...**

In the solution, there are 2 back end Apis : Weather API and Stock API.

The Api Orchestration is set up as shown below.

```C#
    public static class ApiOrchestration
    {
        public static void Create(IApiOrchestrator apis, IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            var weatherService = serviceProvider.GetService<IWeatherService>();

            var weatherApiClientConfig = weatherService.GetClient();

            apis.AddApi("weatherservice", "http://localhost:58262/")
                        .AddRoute("forecast", new RouteInfo { Path = "weatherforecast/forecast", ResponseType = typeof(IEnumerable<WeatherForecast>) })
                        .AddRoute("types", new RouteInfo { Path = "weatherforecast/types", ResponseType = typeof(string[]), HttpClientConfig = weatherApiClientConfig })
                        .AddRoute("type", new RouteInfo { Path = "weatherforecast/types/", ResponseType = typeof(WeatherTypeResponse), HttpClientConfig = weatherApiClientConfig })
                        .AddRoute("test", weatherService.GetTypes)
                    .ToOrchestrator()
                .AddApi("stockservice", "http://localhost:58352/")
                        .AddRoute("stocks", new RouteInfo { Path = "stock", ResponseType = typeof(IEnumerable<StockQuote>) })
                        .AddRoute("stock", new RouteInfo { Path = "stock/", ResponseType = typeof(StockQuote) });
        }
    }
```

The Gateway Swagger appears as shown below:

![API Gateway Swagger](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/ApiGateway.PNG)

To call the forecast route on the weather service,

you can enter the Api key and Route key into Swagger as below:

![API Gateway Swagger](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/ApiGatewayCall.PNG)

This will hit the **weatherforecast/forecast** endpoint on the backend Weather API.

