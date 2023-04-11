### Customizations

You may want to customize your calls to the back end Apis.

You can customize the default **HttpClient** used by all the routes, to hit the backend Api.

```C#
    //Api gateway
    services.AddApiGateway(options =>
    {
        options.DefaultHttpClientConfigure = (sp, httpClient) => httpClient.DefaultRequestHeaders.Add("My header", "My header value");
    });
```

Also, the library provides hooks to

*   Customize the default HttpClient which each route uses to hit the backend Api.
*	Use your own **HttpClient** for each route.
*	Use your own custom implementation to hit the backend Api.

For eg.

These hooks are implemented in your Gateway API project (eg. WeatherService below).

```C#
    public class WeatherService : IWeatherService
    {
        /// <summary>
        /// If you want to customize the default HttpClient or
        /// use your own custom HttpClient or HttpContent 
        /// to hit the backend Api call, you can do this.
        /// </summary>
        /// <returns><see cref="HttpClientConfig"/></returns>
        public HttpClientConfig GetClientConfig()
        {
            return new HttpClientConfig()
            {
                //customize the default HttpClient. eg. add a header.
                CustomizeDefaultHttpClient = (httpClient, request) => httpClient.DefaultRequestHeaders.Add("My header", "My header value"), 
                //OR
                //your own custom HttpClient
                HttpClient = () => new HttpClient()
            };
        }

        /// <summary>
        /// If you want to completely customize your backend Api call, you can do this
        /// </summary>
        /// <param name="apiInfo">The api info</param>
        /// <param name="request">The gateway's incoming request</param>
        /// <returns></returns>
        public async Task<object> GetForecast(ApiInfo apiInfo, HttpRequest request)
        {
            //Create your own implementation to hit the backend.
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{apiInfo.BaseUrl}weatherforecast/forecast");

                response.EnsureSuccessStatusCode();

                return JsonConvert.DeserializeObject<WeatherForecast[]>(await response.Content.ReadAsStringAsync());
            }
        }
    }
```

See **HttpRequest** [here](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httprequest?view=aspnetcore-6.0).

Wire up the WeatherService for injection.

```C#
	services.AddTransient<IWeatherService, WeatherService>();
```

Then, they are hooked up to **routes** in the **Api Orchestrator**.

```C#
    public static class ApiOrchestration
    {
        public static void Create(IApiOrchestrator orchestrator, IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            var weatherService = serviceProvider.GetService<IWeatherService>();

            var weatherApiClientConfig = weatherService.GetClientConfig();

            orchestrator.AddApi("weatherservice", "http://localhost:63969/")                                
                                //Get using customize default HttpClient or your own custom HttpClient
                                .AddRoute("types", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/types", ResponseType = typeof(string[]), HttpClientConfig = weatherApiClientConfig })
                                //Get using custom implementation
                                .AddRoute("forecast-custom", GatewayVerb.GET, weatherService.GetForecast);
        }
    }
```


## Request aggregation

Your Api Gateway gets one incoming request.

Then, you can make multiple calls to back end, downstream Apis and aggregate their responses.

You can do this in the **custom implementation**.