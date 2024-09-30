### Api Gateway Config Settings

You can keep the **ApiKey**, **RouteKey**, **Backend API Base Urls** and **Route Paths**,

in your appsettings.json file.

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

You can read this information, using a **Config Service** like below:

```C#
using AspNetCore.ApiGateway;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Gateway.API
{
    public class ApiSetting
    {
        public string Identifier { get; set; }
        public string ApiKey { get; set; }
        public string[] BackendAPIBaseUrls { get; set; }
        public RouteSetting[] Routes { get; set; }
        public RouteSetting this[string routeIdentifier]
        {
            get
            {
                return Routes.Single(r => r.Identifier == routeIdentifier);
            }
        }        
    }

    public class RouteSetting
    {
        public string Identifier { get; set; }
        public string RouteKey { get; set; }
        public GatewayVerb Verb { get; set; }
        public string BackendAPIRoutePath { get; set; }
        public int ResponseCachingDurationInSeconds { get; set; } = -1;
    }

    public interface IConfigService
    {
        ApiSetting this[string identifier] { get; }            
    }

    public class ConfigService : IConfigService
    {
        private IEnumerable<ApiSetting> Settings { get; set; }

        public ConfigService(IConfiguration configuration)
        {
            var settings = configuration.GetSection("Settings")
                                        .Get<List<ApiSetting>>();

            this.Settings = settings;
        }

        public ApiSetting this[string identifier]
        {
            get
            {
                return Settings.Single(s => s.Identifier == identifier);
            }
        }        
    }

    public static class ConfigProvider
    {
        public static IConfigService MySettings { get; set; }       
    }
}
```

Wire up the Config Service for dependency injecton.

```C#
services.AddSingleton<IConfigService, ConfigService>();
```

Create a static **Settings** class like below:

```C#
public static class Settings
{
    private static IConfigService _settings = ConfigProvider.MySettings;

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

Then, in the Create method, first set the **ConfigProvider** 

and then you can pass these Settings to the **Api Orchestrator**

and where ever else you need them.

```C#
var settings = serviceProvider.GetRequiredService<IConfigService>();
ConfigProvider.MySettings = settings;

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