## Api Gateway Client (Typescript)

|Packages|Version & Downloads|
|---------------------------|:---:|
|*ts-aspnetcore-apigateway-client*|[![NPM Downloads count](https://img.shields.io/npm/dw/ts-aspnetcore-apigateway-client)](https://www.npmjs.com/package/ts-aspnetcore-apigateway-client)|

A  Web client application can talk to the Api Gateway using the TypeScript Client.

The Client is available in project folder **ts-aspnetcore-apigateway-client**, of the solution.

The Client uses [node-fetch](https://www.npmjs.com/package/node-fetch) library under the covers.

[TypeScript Client Integraton Tests](/ts-aspnetcore-apigateway-client/tests/apigatewayclient.test.ts)

## Instructions

1. Add the package to your project from NPM.

```javascript
npm i ts-aspnetcore-apigateway-client
```
2. Import the required classes

```javascript
import { Dictionary } from "ts-aspnetcore-apigateway-client/node_modules/ts-generic-collections-linq";

import { ApiGatewayClient, ApiGatewayClientSettings, ApiGatewayParameters, JsonPatchOperation, Operation} from "ts-aspnetcore-apigateway-client"
```

2. Use the Client.

```javascript
  it('get', async function() {
    let settings = new ApiGatewayClientSettings();
    settings.ApiGatewayBaseUrl = "https://localhost:5001"

    let client = new ApiGatewayClient(settings);

    let headers = new Dictionary<string, string>();
    headers.add("Authorization", "bearer wq298cjwosos==");

    var params = new ApiGatewayParameters();
    params.Api = "weatherservice";
    params.Key = "forecast";
    params.Headers = headers;

    let forecasts = await client.GetAsync<WeatherForecast[]>(params);

    expect(forecasts.length).toBe(5);
  });
```