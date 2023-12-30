### Api Gateway Exception Filters

The library provides interfaces your Gateway API can implement to hook into the endpoint exception filters.

Here you can handle exceptions thrown by the endpoints.

In your Gateway API project,

### you can hook into a common exception filter by implementing the below interface.

```C#
Task OnExceptionAsync(ExceptionContext context, string apiKey, string routeKey, string verb);
```

See **ExceptionContext** [here](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.exceptioncontext?view=aspnetcore-6.0).

### Example

In your Gateway API project,

*	Create a service like below

```C#
    public class ExceptionFilterService : IGatewayExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext context, string apiKey, string routeKey, string verb)
        {
            //handle exception here

            await Task.CompletedTask;
        }
    }
```

*	Wire it up for dependency injection in Startup.cs

```C#
services.AddScoped<IGatewayExceptionFilter, ExceptionFilterService>();
.
.
services.AddApiGateway();
```

### you can hook into each verb's endpoint exception filter by implementing the below interfaces.

```C#
Task OnExceptionAsync(ExceptionContext context, string apiKey, string routeKey);
```

### GET / HEAD

*	IGetOrHeadGatewayExceptionFilter

### POST

*	IPostGatewayExceptionFilter

### PUT

*	IPutGatewayExceptionFilter

### PATCH

*	IPatchGatewayExceptionFilter

### DELETE

*	IDeleteGatewayExceptionFilter

### GET Orchestration

*	IGetOrchestrationGatewayExceptionFilter

### POST Hub

*	IHubPostGatewayExceptionFilter


### Example

In your Gateway API project,

*	Create a service like below

```C#
    public class PostExceptionFilterService : IPostGatewayExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext context, string apiKey, string routeKey)
        {
            //handle exception here

            await Task.CompletedTask;
        }
    }
```

*	Wire it up for dependency injection in Startup.cs

```C#
services.AddScoped<IPostGatewayExceptionFilter, PostExceptionFilterService>();
.
.
services.AddApiGateway();
```