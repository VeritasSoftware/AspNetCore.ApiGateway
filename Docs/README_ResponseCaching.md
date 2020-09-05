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

### Response Cache Settings

| Setting | Description |
| ------- | ----------- |
|Duration | Duration in **seconds** for which the response is cached. This sets "max-age" in "Cache-control" header.|
|Location | Location where the data from a particular URL must be cached.|
|VaryByHeader | Value for the Vary response header. |
|VaryByQueryKeys | Query keys to vary by. |
|NoStore | Value which determines whether the data should be stored or not. When set to true, it sets "Cache-control" header to "no-store". Ignores the "Location" parameter for values other than "None". Ignores the "duration" parameter.|