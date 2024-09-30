### Api Gateway Response Caching

You can confiure your Gateway Api to cache responses.

```C#
    //Api gateway
    services.AddApiGateway(options =>
    {
        options.UseResponseCaching = true;
        options.ResponseCacheSettings = new ApiGatewayResponseCacheSettings
        {
            Duration = 60, //default for all routes
            Location = ResponseCacheLocation.Any,
            //Use VaryByQueryKeys to vary the response for each apiKey & routeKey
            VaryByQueryKeys = new[] { "apiKey", "routeKey" } 
        };
    });
```

### Varying Duration based on apiKey, routeKey

The Duration specified above will be the default for all routes.

For varying the Duration based on Route, in the Api Orchestrator Route, specify **ResponseCachingDurationInSeconds**.

```C#
orchestrator.AddApi("stockservice", "https://localhost:5005/")
                .AddRoute("stocks", GatewayVerb.GET, new RouteInfo { Path = "stock", ResponseType = typeof(IEnumerable<StockQuote>), ResponseCachingDurationInSeconds = 120 })
```

If ResponseCachingDurationInSeconds is specified, it will override the default and will be applied for that route.

ResponseCachingDurationInSeconds should be 0 or greater.

### Response Cache Settings

| Setting | Type | Description |
| ------- | ------- | ----------- |
|Duration | int | Duration in **seconds** for which the response is cached. This sets "max-age" in "Cache-control" header.|
|Location | ResponseCacheLocation | Location where the data from a particular URL must be cached.|
|VaryByHeader | string | Value for the Vary response header. |
|VaryByQueryKeys | string[] | Query keys to vary by. |
|NoStore | bool | Value which determines whether the data should be stored or not. When set to true, it sets "Cache-control" header to "no-store". Ignores the "Location" parameter for values other than "None". Ignores the "duration" parameter.|
| CacheProfileName | string |  Value of the cache profile name. |

The response caching is done by the [**ResponseCacheAttribute**](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.responsecacheattribute?view=aspnetcore-3.0).
