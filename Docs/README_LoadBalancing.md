### Api Gateway Load Balancing

You can set up load balancing in your Gateway API.

You can do this by providing **multiple base urls** to the AddApi method of the Api Orchestrator.

The default algorithm for selecting the base url is **Random**.

```c#
orchestrator.AddApi("weatherservice", "http://localhost:63969/", "http://localhost:63970/")
                    //Get
                    .AddRoute("forecast", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/forecast", ResponseType = typeof(IEnumerable<WeatherForecast>) })
```

However, **Round Robin** is also supported.

```c#
orchestrator.AddApi("weatherservice", LoadBalancingType.RoundRobin, "http://localhost:63969/", "http://localhost:63970/")
                    //Get
                    .AddRoute("forecast", GatewayVerb.GET, new RouteInfo { Path = "weatherforecast/forecast", ResponseType = typeof(IEnumerable<WeatherForecast>) })
```

In this algorithm, the base url is selected in a **Round Robin** way.