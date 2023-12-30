### Api Gateway Authorization

The library provides interfaces your Gateway API can implement to hook into the endpoint authorization.

In your Gateway API project, hook up your Authentication and Authorization as you want.

You can now authorize using the **Gateway Authorization Filters** as shown below.

In your Gateway API project,

### you can hook into a common authorization by implementing the below interface.

```C#
Task AuthorizeAsync(AuthorizationFilterContext context, string apiKey, string routeKey, string verb);
```

See **AuthorizationFilterContext** [here](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.authorizationfiltercontext?view=aspnetcore-6.0).

### Example

In your Gateway API project,

*	Create a service like below

```C#
    public class AuthorizationService : IGatewayAuthorization
    {
        public async Task AuthorizeAsync(AuthorizationFilterContext context, string apiKey, string routeKey, string verb)
        {
            //Put your authorization here

            await Task.CompletedTask;
        }
    }
```

*	Wire it up for dependency injection in Startup.cs

```C#
services.AddScoped<IGatewayAuthorization, AuthorizationService>();
.
.
services.AddApiGateway();
```

### you can hook into each verb's endpoint authorization by implementing the below interfaces.

```C#
Task AuthorizeAsync(AuthorizationFilterContext context, string apiKey, string routeKey)
```

### GET / HEAD

*	IGetOrHeadGatewayAuthorization

### POST

*	IPostGatewayAuthorization

### PUT

*	IPutGatewayAuthorization

### PATCH

*	IPatchGatewayAuthorization

### DELETE

*	IDeleteGatewayAuthorization

### GET Orchestration

*	IGetOrchestrationGatewayAuthorization

### Example

In your Gateway API project,

*	Create a service like below

```C#
    public class GetAuthorizationService : IGetOrHeadGatewayAuthorization
    {
        public async Task AuthorizeAsync(AuthorizationFilterContext context, string apiKey, string routeKey)
        {
            //Put your authorization here

            await Task.CompletedTask;
        }
    }
```

*	Wire it up for dependency injection in Startup.cs

```C#
services.AddScoped<IGetOrHeadGatewayAuthorization, GetAuthorizationService>();
.
.
services.AddApiGateway();
```