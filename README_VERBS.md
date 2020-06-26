### GET

![API Gateway Swagger](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/ApiGatewayCall.PNG)

### HEAD

![API Gateway Swagger](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/HEAD.PNG)

### GET with Params

![API Gateway Swagger](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/GETWithParams.PNG)

You can specify your entire route in the **parameters** query string param.

eg. you may want to hit

GET http://localhost:63990/student/{year}/subject/{code} 

route on the back end API.

You will set up the **Orchestration** as

```
    orchestrator.AddApi("schoolservice", "http://localhost:63990/")
                        .AddRoute("year-subject", GatewayVerb.GET, new RouteInfo { Path = "student/" });
```

And call the GET endpoint as

![API Gateway Swagger](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/GETWithParams1.PNG)

### POST

![API Gateway Swagger](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/POST.PNG)

### PUT

![API Gateway Swagger](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/Update.PNG)

### PATCH

![API Gateway Swagger](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/Patch.PNG)

### DELETE

![API Gateway Swagger](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/Delete.PNG)

### GET Orchestration

![API Gateway Swagger](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/Orchestration.PNG)