import { ApiGatewayClient, ApiGatewayClientSettings, ApiGatewayHeaders, ApiGatewayParameters, JsonPatchOperation, Operation } from "../src"

//Below import is for running compiled code in build folder.
//To compile the project run: npx tsc in the terminal window.

//import { ApiGatewayClient, ApiGatewayClientSettings, ApiGatewayHeaders, ApiGatewayParameters, JsonPatchOperation, Operation } from "../build"

describe('Api Gateway Client Tests', function() {
    it('get', async function() {
        let settings = new ApiGatewayClientSettings();
        settings.ApiGatewayBaseUrl = "https://localhost:5001"
        settings.IsDEVMode = true;

        let client = new ApiGatewayClient(settings);

        let headers = new ApiGatewayHeaders();
        headers.add("Authorization", "bearer wq298cjwosos==");

        var params = new ApiGatewayParameters();
        params.ApiKey = "weatherservice";
        params.RouteKey = "forecast";
        params.Headers = headers;

        let forecasts = await client.GetAsync<WeatherForecast[]>(params);

        expect(forecasts.length).toBe(5);
    });

    it('getWithParams', async function() {
        let settings = new ApiGatewayClientSettings();
        settings.ApiGatewayBaseUrl = "https://localhost:5001"
        settings.IsDEVMode = true;

        let client = new ApiGatewayClient(settings);

        let headers = new ApiGatewayHeaders();
        headers.add("Authorization", "bearer wq298cjwosos==");

        var params = new ApiGatewayParameters();
        params.ApiKey = "weatherservice";
        params.RouteKey = "type";
        params.Parameters = "3";
        params.Headers = headers;

        let weatherType = await client.GetAsync<WeatherTypeResponse>(params);

        expect(weatherType.type).toBe("Cool");

        params = new ApiGatewayParameters();
        params.ApiKey = "weatherservice";
        params.RouteKey = "typewithparams";
        params.Parameters = "index=3";
        params.Headers = headers;

        weatherType = await client.GetAsync<WeatherTypeResponse>(params);

        expect(weatherType.type).toBe("Cool");
    });    

    it('post', async function() {
        let settings = new ApiGatewayClientSettings();
        settings.ApiGatewayBaseUrl = "https://localhost:5001"
        settings.IsDEVMode = true;

        let client = new ApiGatewayClient(settings);

        let headers = new ApiGatewayHeaders();
        headers.add("Authorization", "bearer wq298cjwosos==");

        var params = new ApiGatewayParameters();
        params.ApiKey = "weatherservice";
        params.RouteKey = "add";
        params.Headers = headers;
        
        let payload = new AddWeatherTypeRequest();
        payload.weatherType = "Windy";

        let weatherTypes = await client.PostAsync<AddWeatherTypeRequest, string[]>(params, payload);

        expect(weatherTypes).not.toBe(null);
        if (weatherTypes)
            expect(weatherTypes[weatherTypes.length - 1]).toBe("Windy");
    });
    
    it('put', async function() {
        let settings = new ApiGatewayClientSettings();
        settings.ApiGatewayBaseUrl = "https://localhost:5001"
        settings.IsDEVMode = true;

        let client = new ApiGatewayClient(settings);

        let headers = new ApiGatewayHeaders();
        headers.add("Authorization", "bearer wq298cjwosos==");

        var params = new ApiGatewayParameters();
        params.ApiKey = "weatherservice";
        params.RouteKey = "update";
        params.Headers = headers;
        
        let payload = new UpdateWeatherTypeRequest();
        payload.weatherType = "Coooooooooooool";
        payload.index = 3;

        await client.PutAsync(params, payload);

        params = new ApiGatewayParameters();
        params.ApiKey = "weatherservice";
        params.RouteKey = "types";
        params.Headers = headers;

        var weatherTypes = await client.GetAsync<string[]>(params);

        expect(weatherTypes[3]).toBe("Coooooooooooool");
    }); 
    
    it('patch', async function() {
        let settings = new ApiGatewayClientSettings();
        settings.ApiGatewayBaseUrl = "https://localhost:5001"
        settings.IsDEVMode = true;

        let client = new ApiGatewayClient(settings);

        let headers = new ApiGatewayHeaders();
        headers.add("Authorization", "bearer wq298cjwosos==");

        var params = new ApiGatewayParameters();
        params.ApiKey = "weatherservice";
        params.RouteKey = "patch";
        params.Headers = headers;

        let jsonPatch = new Array<JsonPatchOperation>();

        var operation = new JsonPatchOperation();
        operation.op = Operation.Add;
        operation.path = "/TemperatureC";
        operation.value = 35;

        jsonPatch.push(operation);

        let weatherForecast = await client.PatchAsync<WeatherForecast>(params, jsonPatch);

        expect(weatherForecast).not.toBe(null);
        if (weatherForecast)
            expect(weatherForecast.temperatureC).toBe(35);
    });
    
    it('delete', async function() {
        let settings = new ApiGatewayClientSettings();
        settings.ApiGatewayBaseUrl = "https://localhost:5001"
        settings.IsDEVMode = true;

        let client = new ApiGatewayClient(settings);

        let headers = new ApiGatewayHeaders();
        headers.add("Authorization", "bearer wq298cjwosos==");

        var params = new ApiGatewayParameters();
        params.ApiKey = "weatherservice";
        params.RouteKey = "remove";
        params.Parameters = "0"; 
        params.Headers = headers;      

        await client.DeleteAsync(params);

        params = new ApiGatewayParameters();
        params.ApiKey = "weatherservice";
        params.RouteKey = "types";
        params.Headers = headers;

        var weatherTypes = await client.GetAsync<string[]>(params);        

        expect(weatherTypes[0]).not.toBe("Freezing");
    }); 
    
    it('orchestration', async function() {
        let settings = new ApiGatewayClientSettings();
        settings.ApiGatewayBaseUrl = "https://localhost:5001"
        settings.IsDEVMode = true;

        let client = new ApiGatewayClient(settings);

        var params = new ApiGatewayParameters();   

        let orchestrations = await client.GetOrchestrationAsync(params);     

        expect(orchestrations.length).toBe(4);
    });     
});

class WeatherForecast {
    date?: Date;
    temperatureC?: number;
    temperatureF?: number;
    summary?: string;
}

class AddWeatherTypeRequest {
    weatherType?: string;
}

class UpdateWeatherTypeRequest {
    weatherType?: string;
    index?: number;
}

class WeatherTypeResponse {
    type?: string;
}