### Api Gateway Config Settings

You can keep the **ApiKey**, **RouteKey**, **Backend API Base Urls** and **Route Paths**,

in your **appsettings.json** file.

The **Identifier** is used to identify each **ApiSetting** & **RouteSetting**.

In your Gateway API project's appsettings.json file, add a **Settings** section like shown below:

```JSON
{
  "Settings": [
    {
      "Identifier": "API1",
      "ApiKey": "weatherservice",
      "BackendAPIBaseUrls": [
        "https://localhost:5003/"
      ],
      "Routes": [
        {
          "Identifier": "ROUTE1",
          "RouteKey": "forecast",
          "Verb":  "GET",
          "BackendAPIRoutePath": "weatherforecast/forecast",
          "ResponseCachingDurationInSeconds": 120
        }
      ]
    },
    {
      "Identifier": "API2",
      "ApiKey": "stockservice",
      "BackendAPIBaseUrls": [
        "https://localhost:5005/"
      ],
      "Routes": [
        {
          "Identifier": "ROUTE1",
          "RouteKey": "stocks",
          "Verb":  "GET",
          "BackendAPIRoutePath": "stock",
          "ResponseCachingDurationInSeconds": 180
        }
      ]
    }
  ],
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

You can read this appsettings, using a **Config Service** like below:

For Version 6.0 or higher, You can use the built-in **ApiGatewayConfigService**.

or you can make a copy of the Service, in your project.

[**Api Gateway Config Service**](../AspNetCore.ApiGateway/Application/ApiGatewayConfigService.cs)

Wire up the Config Service for dependency injecton.

```C#
services.AddSingleton<IApiGatewayConfigService, ApiGatewayConfigService>();
```

Create a static **Settings** class like below:

```C#
public static class Settings
{
    private static IApiGatewayConfigService _settings = ApiGatewayConfigProvider.MySettings;

    private const string API1 = "API1";
    private const string API1_ROUTE1 = "ROUTE1";

    public static string API1_ApiKey = _settings[API1].ApiKey;
    public static string[] API1_BackendAPIBaseUrls = _settings[API1].BackendAPIBaseUrls;

    public static string API1_ROUTE1_RouteKey = _settings[API1][API1_ROUTE1].RouteKey;
    public static GatewayVerb API1_ROUTE1_Verb = _settings[API1][API1_ROUTE1].Verb;
    public static string API1_ROUTE1_BackendAPIRoutePath = _settings[API1][API1_ROUTE1].BackendAPIRoutePath;
    public static int API1_ROUTE1_ResponseCachingDurationInSeconds = _settings[API1][API1_ROUTE1].ResponseCachingDurationInSeconds;
}
```

Then, in the Create method, first intitialize the Config Service.

and then you can pass these Settings to the **Api Orchestrator**

and where ever else you need them.

```C#
//Initialize
var settings = serviceProvider.GetRequiredService<IApiGatewayConfigService>();

orchestrator.AddApi(Settings.API1_ApiKey, Settings.API1_BackendAPIBaseUrls)
                //Get
                .AddRoute(Settings.API1_ROUTE1_RouteKey, Settings.API1_ROUTE1_Verb, new RouteInfo { Path = Settings.API1_ROUTE1_BackendAPIRoutePath, ResponseCachingDurationInSeconds = Settings.API1_ROUTE1_ResponseCachingDurationInSeconds })
```

In the **Filters**, you can use the Settings & do like shown below:

```C#
public class ActionFilterService : IGatewayActionFilter
{    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, string apiKey, string routeKey, string verb)
    {
        if (apiKey == Settings.API1_ApiKey)
        {
            if (routeKey == Settings.API1_ROUTE1_RouteKey)
            {
                //Do your work here for API1 -> ROUTE1
            }
        }

        await Task.CompletedTask;
    }
}
```

### Deployment to Prod

As with any Web API, when there is any code change, the API Gateway too is published and deployed using **Blue/Green deployment**.

This is available in **Azure** & **AWS**.

In Azure App Service, you use **deployment slots** etc. 

*   Read [more](https://learn.microsoft.com/en-us/azure/app-service/deploy-best-practices#use-deployment-slots).
*   Read [more](https://learn.microsoft.com/en-us/azure/container-apps/blue-green-deployment?pivots=azure-cli).

Azure Kubernetes Service (AKS) also supports Blue/Green deployment. Read [more](https://learn.microsoft.com/en-us/azure/architecture/guide/aks/blue-green-deployment-for-aks).

In AWS, you use **Elastic Beanstalk**. Read [more](https://docs.aws.amazon.com/elasticbeanstalk/latest/dg/using-features.CNAMESwap.html).

So, there is no down time.