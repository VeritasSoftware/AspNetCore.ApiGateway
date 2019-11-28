### Api Gateway Authorization

The library provides interfaces your Gateway API can implement to hook into the endpoint authorization.

You have to implement an interface method called **Authorize**.

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