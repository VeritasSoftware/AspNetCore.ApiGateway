### Api Gateway Authorization

The library provides interfaces your Gateway API can implement to hook into the endpoint authorization.

In your Gateway API project,

you have to implement an interface method called **AuthorizeAsync**.

```C#
Task AuthorizeAsync(AuthorizationFilterContext context, string api, string key)
```

### GET

*	IGetGatewayAuthorization

### POST

*	IPostGatewayAuthorization

### PUT

*	IPutGatewayAuthorization

### DELETE

*	IDeleteGatewayAuthorization


### Example

In your Gateway API project,

*	Create a service like below

```C#
    public class GetAuthorizationService : IGetGatewayAuthorization
    {
        public async Task AuthorizeAsync(AuthorizationFilterContext context, string api, string key)
        {
            //Put your authorization here
        }
    }
```

*	Wire it up for dependency injection in Startup.cs

```C#
services.AddScoped<IGetGatewayAuthorization, GetAuthorizationService>();
.
.
services.AddApiGateway();
```
