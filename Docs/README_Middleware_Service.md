### Api Gateway Middleware Service

The library provides an interface your Gateway API can implement to hook into the Gateway's middleware.

In your Gateway API project,

### you can hook into middleware service by implementing the below interface.

```C#
Task Invoke(HttpContext context, string apiKey, string routeKey);
```

### Example

In your Gateway API project,

*	Create a service like below

```C#
    public class GatewayMiddlewareService : IGatewayMiddleware
    {
        public async Task Invoke(HttpContext context, string apiKey, string routeKey)
        {
            //do your work here

            await Task.CompletedTask;
        }
    }
```

*	Wire it up for dependency injection in Startup.cs

```C#
services.AddTransient<IGatewayMiddleware, GatewayMiddlewareService>();
.
.
services.AddApiGateway();
```