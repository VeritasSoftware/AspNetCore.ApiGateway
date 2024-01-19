### Api Gateway Response Caching

You can confiure your Gateway Api to cache responses.

```C#
    //Api gateway
    services.AddApiGateway(options =>
    {
        options.UseResponseCaching = true;
        options.ResponseCacheSettings = new ApiGatewayResponseCacheSettings
        {
            Duration = 120,
            Location = ResponseCacheLocation.Any,
            //Use VaryByQueryKeys to vary the response for each apiKey & routeKey
            VaryByQueryKeys = new[] { "apiKey", "routeKey" } 
        };
    });
```

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