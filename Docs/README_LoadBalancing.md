### Api Gateway Load Balancing

You can set up load balancing in your Gateway API.

You can do this by providing **multiple base urls** to the AddApi method of the Api Orchestrator.

```c#
orchestrator.AddApi("weatherservice", "http://localhost:63969/", "http://localhost:63970/")
                    //Get
                    .AddRoute("forecast", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/forecast", ResponseType = typeof(IEnumerable<WeatherForecast>) })
```

The base url to be used for hitting the backend Api is selected at **Random**.