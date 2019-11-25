using AspNetCore.ApiGateway;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace ApiGateway.API
{
    public static class ApiOrchestration
    {
        public static void Create(IApiOrchestrator orchestrator, IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            var weatherService = serviceProvider.GetService<IWeatherService>();

            var weatherApiClientConfig = weatherService.GetClientConfig();

            orchestrator.AddApi("weatherservice", "http://localhost:58262/")
                                //Get
                                .AddRoute("forecast", new RouteInfo { Path = "weatherforecast/forecast", ResponseType = typeof(IEnumerable<WeatherForecast>) })
                                //Get using custom HttpClient
                                .AddRoute("types", new RouteInfo { Path = "weatherforecast/types", ResponseType = typeof(string[]), HttpClientConfig = weatherApiClientConfig })
                                //Get with param using custom HttpClient
                                .AddRoute("type", new RouteInfo { Path = "weatherforecast/types/", ResponseType = typeof(WeatherTypeResponse), HttpClientConfig = weatherApiClientConfig })
                                //Get using custom implementation
                                .AddRoute("typescustom", weatherService.GetTypes)
                                //Post
                                .AddRoute("add", new RouteInfo { Path = "weatherforecast/types/add", RequestType = typeof(AddWeatherTypeRequest), ResponseType = typeof(string[])})
                                //Put
                                .AddRoute("update", new RouteInfo { Path = "weatherforecast/types/update", RequestType = typeof(UpdateWeatherTypeRequest), ResponseType = typeof(string[]) })
                                //Delete
                                .AddRoute("remove", new RouteInfo { Path = "weatherforecast/types/remove/", ResponseType = typeof(string[]) })
                        .ToOrchestrator()
                        .AddApi("stockservice", "http://localhost:58352/")
                                .AddRoute("stocks", new RouteInfo { Path = "stock", ResponseType = typeof(IEnumerable<StockQuote>) })
                                .AddRoute("stock", new RouteInfo { Path = "stock/", ResponseType = typeof(StockQuote) });
        }
    }
}
