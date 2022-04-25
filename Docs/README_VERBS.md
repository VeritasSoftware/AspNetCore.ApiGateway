### GET

![API Gateway Swagger](/Docs/ApiGatewayCall.PNG)

### HEAD

![API Gateway Swagger](/Docs/HEAD.PNG)

### GET with Params

![API Gateway Swagger](/Docs/GETWithParams.PNG)

You can specify your entire route in the **parameters** query string param.

eg. you may want to hit

GET http://localhost:63990/student/{year}/subject/{code}?division={division}

route on the back end API.

You will set up the **Orchestration** as

```C#
orchestrator.AddApi("schoolservice", "http://localhost:63990/")
                    .AddRoute("year-subject", GatewayVerb.GET, new RouteInfo { Path = "student/" });
```

And call the GET endpoint as

![API Gateway Swagger](/Docs/GETWithParams1.PNG)

**Or** 

### Parameterized route

You will set up a parameterized route in the **Orchestration** as

```C#
orchestrator.AddApi("schoolservice", "http://localhost:63990/")
                    .AddRoute("year-subject", GatewayVerb.GET, new RouteInfo { Path = "student/{year}/subject/{code}?division={division}" })
```

And call the GET endpoint as

![API Gateway Swagger](/Docs/GETWithParams2.PNG)

Any param in the Path (eg. {year}) will be substituted by what is passed in the Parameters field in the request.

So, the Path would become ```student/2020/subject/001?division=A```.

### POST

![API Gateway Swagger](/Docs/POST.PNG)

### PUT

![API Gateway Swagger](/Docs/Update.PNG)

### PATCH

![API Gateway Swagger](/Docs/Patch.PNG)

### DELETE

![API Gateway Swagger](/Docs/Delete.PNG)

### GET Orchestration

![API Gateway Swagger](/Docs/Orchestration.PNG)