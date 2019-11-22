using AspNetCore.ApiGateway;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace ApiGateway.API
{
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
}
