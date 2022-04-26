using AspNetCore.ApiGateway.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using GatewayAPI = ApiGateway.API;
using StockAPI = Stock.API;
using WeatherAPI = Weather.API;

namespace AspNetCore.ApiGateway.Tests
{
    public class APIInitializeClient
    { 

        public APIInitializeClient()
        {
            IWebHostBuilder weatherAPI = new WebHostBuilder()
                                     .UseStartup<WeatherAPI.Startup>()
                                     .UseKestrel(options => options.Listen(IPAddress.Any, 63969));

            weatherAPI.Start();

            IWebHostBuilder stockAPI = new WebHostBuilder()
                                     .UseStartup<StockAPI.Startup>()
                                     .UseKestrel(options => options.Listen(IPAddress.Any, 63967));

            stockAPI.Start();

            IWebHostBuilder gatewayAPI = new WebHostBuilder()
                                     .UseStartup<GatewayAPI.Startup>()
                                     .UseKestrel(options => options.Listen(IPAddress.Any, 80));

            gatewayAPI.Start();
        }
    }

    public class GatewayClientTests : IClassFixture<APIInitializeClient>
    {
        private readonly IServiceProvider _serviceProvider;
        readonly APIInitializeClient _apiInit;

        public GatewayClientTests(APIInitializeClient apiInit)
        {
            _apiInit = apiInit;

            IServiceCollection services = new ServiceCollection();

            services.AddApiGatewayClient(settings => settings.ApiGatewayUrl = "http://localhost/api/Gateway");

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task Test_Get_Pass()
        {
            var client = _serviceProvider.GetRequiredService<IApiGatewayClient>();

            var parameters = new ApiGatewayParameters
            {
                Api = "weatherservice",
                Key = "forecast",
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", "bearer wq298cjwosos==" }
                }
            };

            var forecasts = await client.GetAsync<WeatherForecast[]>(parameters);

            Assert.True(forecasts.Length > 0);
        }

        [Fact]
        public async Task Test_Get_WithParam_Pass()
        {
            var client = _serviceProvider.GetRequiredService<IApiGatewayClient>();

            var parameters = new ApiGatewayParameters
            {
                Api = "weatherservice",
                Key = "type",
                Parameters = "3",
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", "bearer wq298cjwosos==" }
                }
            };

            var weatherType = await client.GetAsync<WeatherTypeResponse>(parameters);

            Assert.NotNull(weatherType);
            Assert.True(!string.IsNullOrEmpty(weatherType.Type));

            parameters = new ApiGatewayParameters
            {
                Api = "weatherservice",
                Key = "typewithparams",
                Parameters = "index=3"
            };

            weatherType = await client.GetAsync<WeatherTypeResponse>(parameters);

            Assert.NotNull(weatherType);
            Assert.True(!string.IsNullOrEmpty(weatherType.Type));
        }

        [Fact]
        public async Task Test_Post_Pass()
        {
            var client = _serviceProvider.GetRequiredService<IApiGatewayClient>();

            AddWeatherTypeRequest payload = new AddWeatherTypeRequest
            {
                WeatherType = "Windy"
            };

            var parameters = new ApiGatewayParameters
            {
                Api = "weatherservice",
                Key = "add",
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", "bearer wq298cjwosos==" }
                }
            };

            var weatherTypes = await client.PostAsync<AddWeatherTypeRequest, string[]>(parameters, payload);

            Assert.True(weatherTypes.Last() == "Windy");
        }

        [Fact]
        public async Task Test_Put_Pass()
        {
            var client = _serviceProvider.GetRequiredService<IApiGatewayClient>();

            UpdateWeatherTypeRequest payload = new UpdateWeatherTypeRequest
            {
                WeatherType = "Coooooooool",
                Index = 3
            };

            var parameters = new ApiGatewayParameters
            {
                Api = "weatherservice",
                Key = "update",
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", "bearer wq298cjwosos==" }
                }
            };

            await client.PutAsync<UpdateWeatherTypeRequest, string>(parameters, payload);

            parameters = new ApiGatewayParameters
            {
                Api = "weatherservice",
                Key = "types",
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", "bearer wq298cjwosos==" }
                }
            };

            var weatherTypes = await client.GetAsync<string[]>(parameters);

            Assert.True(weatherTypes[3] == "Coooooooool");
        }

        [Fact]
        public async Task Test_Patch_Pass()
        {
            var client = _serviceProvider.GetRequiredService<IApiGatewayClient>();

            JsonPatchDocument<WeatherForecast> jsonPatch = new JsonPatchDocument<WeatherForecast>();
            jsonPatch.Add(x => x.TemperatureC, 35);

            var parameters = new ApiGatewayParameters
            {
                Api = "weatherservice",
                Key = "patch",
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", "bearer wq298cjwosos==" }
                }
            };

            var weatherForecast = await client.PatchAsync<WeatherForecast, WeatherForecast>(parameters, jsonPatch);

            Assert.True(weatherForecast.TemperatureC == 35);
        }

        [Fact]
        public async Task Test_Delete_Pass()
        {
            var client = _serviceProvider.GetRequiredService<IApiGatewayClient>();

            var parameters = new ApiGatewayParameters
            {
                Api = "weatherservice",
                Key = "remove",
                Parameters = "0",
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", "bearer wq298cjwosos==" }
                }
            };

            await client.DeleteAsync<string>(parameters);

            parameters = new ApiGatewayParameters
            {
                Api = "weatherservice",
                Key = "types",
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", "bearer wq298cjwosos==" }
                }
            };

            var weatherTypes = await client.GetAsync<string[]>(parameters);

            Assert.DoesNotContain(weatherTypes, x => x == "Freezing");
        }
    }
}
