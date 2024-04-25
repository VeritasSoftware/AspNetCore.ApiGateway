### Api Gateway Action Filters

The library provides interfaces your Gateway API can implement to hook into the endpoint action filters.

Here you can do things like **validation**, **rate limiting**, **logging** etc, if you want.

In your Gateway API project,

### you can hook into a common action filter by implementing the below interface.

```C#
Task OnActionExecutionAsync(ActionExecutingContext context, string apiKey, string routeKey, string verb);
```

See **ActionExecutingContext** [here](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.actionexecutingcontext?view=aspnetcore-6.0).

### Example

In your Gateway API project,

*	Create a service like below

```C#
public class ActionFilterService : IGatewayActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, string apiKey, string routeKey, string verb)
    {
        //do your work here eg. validation

        //set the result, eg below commented line
        //context.Result = new BadRequestObjectResult(context.ModelState);

        await Task.CompletedTask;
    }
}
```

*	Wire it up for dependency injection in Startup.cs

```C#
services.AddScoped<IGatewayActionFilter, ActionFilterService>();
.
.
services.AddApiGateway();
```

### you can hook into each verb's endpoint action filter by implementing the below interfaces.

```C#
Task OnActionExecutionAsync(ActionExecutingContext context, string apiKey, string routeKey);
```

### GET / HEAD

*	IGetOrHeadGatewayActionFilter

### POST

*	IPostGatewayActionFilter

### PUT

*	IPutGatewayActionFilter

### PATCH

*	IPatchGatewayActionFilter

### DELETE

*	IDeleteGatewayActionFilter

### GET Orchestration

*	IGetOrchestrationGatewayActionFilter

### POST Hub

*	IHubPostGatewayActionFilter


### Example

In your Gateway API project,

*	Create a service like below

```C#
public class PostActionFilterService : IPostGatewayActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, string apiKey, string routeKey)
    {
        //do your work here eg. validation

        //set the result, eg below commented line
        //context.Result = new BadRequestObjectResult(context.ModelState);

        await Task.CompletedTask;
    }
}
```

*	Wire it up for dependency injection in Startup.cs

```C#
services.AddScoped<IPostGatewayActionFilter, PostActionFilterService>();
.
.
services.AddApiGateway();
```