### Api Gateway Result Filters

The library provides interfaces your Gateway API can implement to hook into the endpoint result filters.

Here you can modify results of the endpoints.

In your Gateway API project,

### you can hook into a common result filter by implementing the below interface.

```C#
Task OnResultExecutionAsync(ResultExecutingContext context, string apiKey, string routeKey, string verb);
```

See **ResultExecutingContext** [here](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.resultexecutingcontext?view=aspnetcore-6.0).

### Example

In your Gateway API project,

*	Create a service like below

```C#
    public class ResultFilterService : IGatewayResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, string apiKey, string routeKey, string verb)
        {
            //modify result here

            await Task.CompletedTask;
        }
    }
```

*	Wire it up for dependency injection in Startup.cs

```C#
services.AddScoped<IGatewayResultFilter, ResultFilterService>();
.
.
services.AddApiGateway();
```

### you can hook into each verb's endpoint result filter by implementing the below interfaces.

```C#
Task OnResultExecutionAsync(ResultExecutingContext context, string apiKey, string routeKey);
```

### GET / HEAD

*	IGetOrHeadGatewayResultFilter

### POST

*	IPostGatewayResultFilter

### PUT

*	IPutGatewayResultFilter

### PATCH

*	IPatchGatewayResultFilter

### DELETE

*	IDeleteGatewayResultFilter

### GET Orchestration

*	IGetOrchestrationGatewayResultFilter

### POST Hub

*	IHubPostGatewayResultFilter


### Example

In your Gateway API project,

*	Create a service like below

```C#
    public class PostResultFilterService : IPostGatewayResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, string apiKey, string routeKey)
        {
            //modify result here

            await Task.CompletedTask;
        }
    }
```

*	Wire it up for dependency injection in Startup.cs

```C#
services.AddScoped<IPostGatewayResultFilter, PostResultFilterService>();
.
.
services.AddApiGateway();
```