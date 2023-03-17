import { Dictionary } from "ts-generic-collections-linq";
import { ApiGatewayClient } from "../src/ApiGatewayClient";
import { ApiGatewayClientSettings } from "../src/ApiGatewayClientSettings";
import { ApiGatewayParameters } from "../src/ApiGatewayParameters";
import { JsonPatchOperation, Operation } from "../src/JsonPatch";

describe('Api Gateway Client Tests', function() {
    it('get', async function() {
        let settings = new ApiGatewayClientSettings();
        settings.ApiGatewayBaseUrl = "https://localhost:5001"
        settings.UseHttps = true;
        settings.IsDEVMode = true;

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

    it('post', async function() {
        let settings = new ApiGatewayClientSettings();
        settings.ApiGatewayBaseUrl = "https://localhost:5001"
        settings.UseHttps = true;
        settings.IsDEVMode = true;

        let client = new ApiGatewayClient(settings);

        let headers = new Dictionary<string, string>();
        headers.add("Authorization", "bearer wq298cjwosos==");

        var params = new ApiGatewayParameters();
        params.Api = "weatherservice";
        params.Key = "add";
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
        settings.UseHttps = true;
        settings.IsDEVMode = true;

        let client = new ApiGatewayClient(settings);

        let headers = new Dictionary<string, string>();
        headers.add("Authorization", "bearer wq298cjwosos==");

        var params = new ApiGatewayParameters();
        params.Api = "weatherservice";
        params.Key = "update";
        params.Headers = headers;
        
        let payload = new UpdateWeatherTypeRequest();
        payload.weatherType = "Coooooooooooool";
        payload.index = 3;

        await client.PutAsync(params, payload);

        params = new ApiGatewayParameters();
        params.Api = "weatherservice";
        params.Key = "types";
        params.Headers = headers;

        var weatherTypes = await client.GetAsync<string[]>(params);

        expect(weatherTypes[3]).toBe("Coooooooooooool");
    }); 
    
    it('patch', async function() {
        let settings = new ApiGatewayClientSettings();
        settings.ApiGatewayBaseUrl = "https://localhost:5001"
        settings.UseHttps = true;
        settings.IsDEVMode = true;

        let client = new ApiGatewayClient(settings);

        let headers = new Dictionary<string, string>();
        headers.add("Authorization", "bearer wq298cjwosos==");

        var params = new ApiGatewayParameters();
        params.Api = "weatherservice";
        params.Key = "patch";
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
        settings.UseHttps = true;
        settings.IsDEVMode = true;

        let client = new ApiGatewayClient(settings);

        let headers = new Dictionary<string, string>();
        headers.add("Authorization", "bearer wq298cjwosos==");

        var params = new ApiGatewayParameters();
        params.Api = "weatherservice";
        params.Key = "remove";
        params.Parameters = "0"; 
        params.Headers = headers;      

        await client.DeleteAsync(params);

        params = new ApiGatewayParameters();
        params.Api = "weatherservice";
        params.Key = "types";
        params.Headers = headers;

        var weatherTypes = await client.GetAsync<string[]>(params);        

        expect(weatherTypes[0]).not.toBe("Freezing");
    }); 
    
    it('orchestration', async function() {
        let settings = new ApiGatewayClientSettings();
        settings.ApiGatewayBaseUrl = "https://localhost:5001"
        settings.UseHttps = true;
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