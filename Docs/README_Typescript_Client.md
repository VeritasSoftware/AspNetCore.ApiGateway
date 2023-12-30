## Api Gateway Client (Typescript)

|Packages|Version & Downloads|
|---------------------------|:---:|
|*ts-aspnetcore-apigateway-client*|[![NPM Downloads count](https://img.shields.io/npm/dw/ts-aspnetcore-apigateway-client)](https://www.npmjs.com/package/ts-aspnetcore-apigateway-client)|

A  Web client application can talk to the Api Gateway using the TypeScript Client.

The Client is available in project folder **ts-aspnetcore-apigateway-client**, of the solution.

The Client uses [node-fetch](https://www.npmjs.com/package/node-fetch) library under the covers.

[TypeScript Client Integraton Tests](/ts-aspnetcore-apigateway-client/tests/apigatewayclient.test.ts)

## Instructions

1. Install the package in your project, from NPM.

```javascript
npm i ts-aspnetcore-apigateway-client
```

2. Import the required classes.

```javascript
import { ApiGatewayClient, ApiGatewayClientSettings, ApiGatewayHeaders, 
            ApiGatewayParameters, JsonPatchOperation, Operation } from "ts-aspnetcore-apigateway-client";
```

3. Use the Client.

```javascript
  it('get', async function() {
    let settings = new ApiGatewayClientSettings();
    settings.ApiGatewayBaseUrl = "https://localhost:5001";

    let client = new ApiGatewayClient(settings);

    let headers = new ApiGatewayHeaders();
    headers.add("Authorization", "bearer wq298cjwosos==");

    let params = new ApiGatewayParameters();
    params.ApiKey = "weatherservice";
    params.RouteKey = "forecast";
    params.Headers = headers;

    let forecasts = await client.GetAsync<WeatherForecast[]>(params);

    expect(forecasts.length).toBe(5);
  });
```