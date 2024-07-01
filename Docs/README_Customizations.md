# Customizations

## Customize HttpClient

You may want to customize your calls to the back end Apis.

You can customize the default **HttpClient** used by **all the routes**, to hit the backend Api.

```C#
    //Api gateway
    services.AddApiGateway(options =>
    {
        options.DefaultHttpClientConfigure = (sp, httpClient) => httpClient.DefaultRequestHeaders.Add("My header", "My header value");
    });
```

You can create the default **HttpClient** used by **all the routes**, to hit the backend Api.

Eg. below code adds Proxy info to a new HttpClient.

```C#
    var proxyHost = "http://localhost";
    var proxyPort = 80;
    var proxyUserName = "testUser";
    var proxyPassword = "testPwd";

    var proxy = new WebProxy
    {
        Address = new Uri($"http://{proxyHost}:{proxyPort}"),
        BypassProxyOnLocal = false,
        UseDefaultCredentials = false,

        // *** These creds are given to the proxy server, not the web server ***
        Credentials = new NetworkCredential(
            userName: proxyUserName,
            password: proxyPassword)
    };

    var httpClientHandler = new HttpClientHandler
    {
        Proxy = proxy
    };

    //Api gateway
    services.AddApiGateway(options =>
    {
        options.DefaultMyHttpClientHandler = () => httpClientHandler;
    });
```

Also, the library provides hooks to

*   Customize the default HttpClient which **each route** uses to hit the backend Api.
*	Use your own **HttpClient** for each route.

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
    }
```

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
                                .AddRoute("types", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/types", ResponseType = typeof(string[]), HttpClientConfig = weatherApiClientConfig });        }
    }
```

## Custom implementation

*	Use your own custom implementation to hit the backend Api.

Create a Service (like WeatherService).

Add a method (like GetForecast) with the same method signature.
See **HttpRequest** [here](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httprequest?view=aspnetcore-6.0).

In the method, put your custom implementation.

```C#
    public class WeatherService : IWeatherService
    {
        /// <summary>
        /// If you want to completely customize your backend Api call, you can do this
        /// </summary>
        /// <param name="apiInfo">The api info</param>
        /// <param name="request">The gateway's incoming request</param>
        /// <returns>The object to be returned</returns>
        public async Task<object> GetForecast(ApiInfo apiInfo, HttpRequest request)
        {
            //Create your own implementation to hit the backend.
        }
    }
```

Wire up the WeatherService for injection.

```C#
	services.AddTransient<IWeatherService, WeatherService>();
```

Create a Route in the **ApiOrchestrator** as shown below:

```C#
    public static class ApiOrchestration
    {
        public static void Create(IApiOrchestrator orchestrator, IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            var weatherService = serviceProvider.GetService<IWeatherService>();

            orchestrator.AddApi("weatherservice", "http://localhost:63969/")                                
                                //Get using custom implementation
                                .AddRoute("forecast-custom", GatewayVerb.GET, weatherService.GetForecast);
        }
    }
```

## Request aggregation

Your Api Gateway gets one incoming request.

Then, you can make multiple requests to back end, downstream Apis and aggregate their responses.

You can do this in a **custom implementation**.

Read [**more**](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/issues/7)