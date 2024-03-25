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
```

Wire up the Config Service for dependency injecton.

```C#
services.AddSingleton<IConfigService, ConfigService>();
```

Then, in the Create method, you can pass these settings to the **Api Orchestrator**.

```C#
var settings = serviceProvider.GetRequiredService<IConfigService>();

var api1 = settings["API1"];

orchestrator.AddApi(api1.ApiKey, api1.BackendAPIBaseUrls)
                    //Get
                    .AddRoute(api1["ROUTE1"].RouteKey, api1["ROUTE1"].Verb, new RouteInfo { Path = api1["ROUTE1"].BackendAPIRoutePath) })
```