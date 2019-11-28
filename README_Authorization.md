### Api Gateway Authorization

The library provides interfaces your Gateway API can implement to hook into the endpoint authorization.

In your Gateway API project,

you have to implement an interface method called **Authorize**.

```C#
void Authorize(AuthorizationFilterContext context)
```

### GET

*	IGetGatewayAuthorization

### GET with parameters

*	IGetWithParamsGatewayAuthorization

### POST

*	IPostGatewayAuthorization

### PUT

*	IPutGatewayAuthorization

### DELETE

*	IDeleteGatewayAuthorization


### Example

```C#
    public class GetAuthorizationService : IGetGatewayAuthorization
    {
        public void Authorize(AuthorizationFilterContext context)
        {
            //Put your authorization here
        }
    }
```

Wire it up for dependency injection in your Gateway API project's Startup.cs

```C#
services.AddScoped<IGetGatewayAuthorization, GetAuthorizationService>();
.
.
services.AddApiGateway();
```
