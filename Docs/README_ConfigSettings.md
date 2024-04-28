### Api Gateway Config Settings

You can keep the ApiKey, RouteKey, Backend API Base Url and Backend API Route Path,

in your appsettings.json file.

The **Identifier** is used to identify each **ApiSetting**.

In your Gateway API project's appsettings.json file,

add a **Settings** section as shown below:

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
          "BackendAPIRoutePath": "weatherforecast/forecast"
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
          "BackendAPIRoutePath": "stock"
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
    private static IConfigService _settings;

    public static IConfigService MySettings
    {
        get
        {
            return _settings;
        }
        set
        {
            _settings = value;
        }
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

    public static string API1_ApiKey = _settings["API1"].ApiKey;
    public static string[] API1_BackendAPIBaseUrls = _settings["API1"].BackendAPIBaseUrls;

    public static string API1_ROUTE1_RouteKey = _settings["API1"]["ROUTE1"].RouteKey;
    public static GatewayVerb API1_ROUTE1_Verb = _settings["API1"]["ROUTE1"].Verb;
    public static string API1_ROUTE1_BackendAPIRoutePath = _settings["API1"]["ROUTE1"].BackendAPIRoutePath;
}
```


Then, in the Create method, you can pass these settings to the **Api Orchestrator**.

```C#
var settings = serviceProvider.GetRequiredService<IConfigService>();
ConfigProvider.MySettings = settings;

orchestrator.AddApi(Settings.API1_ApiKey, Settings.API1_BackendAPIBaseUrls)
                //Get
                .AddRoute(Settings.API1_ROUTE1_RouteKey, Settings.API1_ROUTE1_Verb, new RouteInfo { Path = Settings.API1_ROUTE1_BackendAPIRoutePath })
```

In the **Filters** you can do as shown below:

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