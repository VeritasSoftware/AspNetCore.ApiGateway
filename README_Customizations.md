### Customizations

You may want to customize your calls to the back end Apis.

The library provides hooks to

*	Use your own **HttpClient**.
*	Use your own custom implementation of the back end Api call.

For eg.

These hooks are implemented in your Gateway API project (eg. WeatherService below).

```C#
    public class WeatherService : IWeatherService
    {
        /// <summary>
        /// If you want to use a custom HttpClient or HttpContent for your backend Api call, you can do this.
        /// </summary>
        /// <returns><see cref="HttpClientConfig"/></returns>
        public HttpClientConfig GetClientConfig()
        {
            return new HttpClientConfig()
            {
                HttpClient = () => new HttpClient()
            };
        }

        /// <summary>
        /// If you want to completely customize your backend Api call, you can do this
        /// </summary>
        /// <param name="apiInfo">The api info</param>
        /// <param name="routeInfo">The route info</param>
        /// <param name="request">The gateway's incoming request</param>
        /// <returns></returns>
        public async Task<object> GetTypes(ApiInfo apiInfo, RouteInfo routeInfo, HttpRequest request)
        {
			//Create your own implementation to hit the back end Api.
        }
    }
```

They are then hooked up to **routes** in the **Api Orchestrator**.

```C#
    public static class ApiOrchestration
    {
        public static void Create(IApiOrchestrator orchestrator, IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            var weatherService = serviceProvider.GetService<IWeatherService>();

            var weatherApiClientConfig = weatherService.GetClientConfig();

            orchestrator.AddApi("weatherservice", "http://localhost:63969/")                                
                                //Get using custom HttpClient
                                .AddRoute("types", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/types", ResponseType = typeof(string[]), HttpClientConfig = weatherApiClientConfig })
                                //Get using custom implementation
                                .AddRoute("typescustom", GatewayVerb.GET, weatherService.GetTypes);
        }
    }
```