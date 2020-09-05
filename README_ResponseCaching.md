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
            Location = ResponseCacheLocation.Any
        };
    });
```