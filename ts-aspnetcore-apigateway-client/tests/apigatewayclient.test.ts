import { ApiGatewayClient } from "../ApiGatewayClient";
import { ApiGatewayClientSettings } from "../ApiGatewayClientSettings";
import { ApiGatewayParameters } from "../ApiGatewayParameters";

describe('Api Gateway Client Tests', function() {
    it('get', async function() {
        let settings = new ApiGatewayClientSettings();
        settings.ApiGatewayBaseUrl = "https://localhost:5001"

        let client = new ApiGatewayClient(settings);

        var params = new ApiGatewayParameters();
        params.Api = "weatherservice";
        params.Key = "forecast";        

        var forecasts = await client.GetAsync<WeatherForecast[]>(params);

        expect(forecasts.length).toBe(5);
    });
});

class WeatherForecast {
    date?: Date;
    temperatureC?: number;
    summary?: string;
}