using AspNetCore.ApiGateway.Client;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Learn.AzureFunctionsTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCore.ApiGateway.Tests
{
    [Collection("Azure Functions Gateway")]
    public class AzureFunctionsGatewayClientTests : IClassFixture<AzureFunctionsAPIInitialize>
    {
        private readonly IServiceProvider _serviceProvider;

        public AzureFunctionsGatewayClientTests(AzureFunctionsAPIInitialize apiInit, FunctionFixture<FunctionStartup> fixture)
        {
            IServiceCollection services = new ServiceCollection();

            //Wire up the Client for dependency injection using extension
            services.AddApiGatewayClient(settings => settings.ApiGatewayBaseUrl = "http://localhost:7055");

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task Test_Get_Pass()
        {
            //Arrange
            
            //Get Client from dependency injection container
            var client = _serviceProvider.GetRequiredService<IApiGatewayClient>();

            var parameters = new ApiGatewayParameters
            {
                ApiKey = "weatherservice",
                RouteKey = "forecast",
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", "bearer wq298cjwosos==" }
                }
            };

            //Act
            var forecasts = await client.GetAsync<WeatherForecast[]>(parameters);

            //Assert
            Assert.True(forecasts.Length > 0);
        }

        [Fact]
        public async Task Test_Get_WithParam_Pass()
        {
            //Arrange

            //Get Client from dependency injection container
            var client = _serviceProvider.GetRequiredService<IApiGatewayClient>();

            var parameters = new ApiGatewayParameters
            {
                ApiKey = "weatherservice",
                RouteKey = "type",
                Parameters = "3",
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", "bearer wq298cjwosos==" }
                }
            };

            //Act
            var weatherType = await client.GetAsync<WeatherTypeResponse>(parameters);

            //Assert
            Assert.NotNull(weatherType);
            Assert.True(!string.IsNullOrEmpty(weatherType.Type));

            ////Arrange
            //parameters = new ApiGatewayParameters
            //{
            //    ApiKey = "weatherservice",
            //    RouteKey = "typewithparams",
            //    Parameters = "index=3",
            //    Headers = new Dictionary<string, string>
            //    {
            //        { "Authorization", "bearer wq298cjwosos==" }
            //    }
            //};

            ////Act
            //weatherType = await client.GetAsync<WeatherTypeResponse>(parameters);

            ////Assert
            //Assert.NotNull(weatherType);
            //Assert.True(!string.IsNullOrEmpty(weatherType.Type));
        }

        [Fact]
        public async Task Test_Post_Pass()
        {
            //Arrange

            //Get Client from dependency injection container
            var client = _serviceProvider.GetRequiredService<IApiGatewayClient>();

            AddWeatherTypeRequest payload = new AddWeatherTypeRequest
            {
                WeatherType = "Windy"
            };
            
            var parameters = new ApiGatewayParameters
            {
                ApiKey = "weatherservice",
                RouteKey = "add",
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", "bearer wq298cjwosos==" }
                }
            };
            
            //Act
            var weatherTypes = await client.PostAsync<AddWeatherTypeRequest, string[]>(parameters, payload);

            //Assert
            Assert.True(weatherTypes.Last() == "Windy");
        }

        [Fact]
        public async Task Test_Put_Pass()
        {
            //Arrange

            //Get Client from dependency injection container
            var client = _serviceProvider.GetRequiredService<IApiGatewayClient>();

            UpdateWeatherTypeRequest payload = new UpdateWeatherTypeRequest
            {
                WeatherType = "Coooooooool",
                Index = 3
            };

            var parameters = new ApiGatewayParameters
            {
                ApiKey = "weatherservice",
                RouteKey = "update",
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", "bearer wq298cjwosos==" }
                }
            };

            //Act
            await client.PutAsync(parameters, payload);

            parameters = new ApiGatewayParameters
            {
                ApiKey = "weatherservice",
                RouteKey = "types",
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", "bearer wq298cjwosos==" }
                }
            };

            var weatherTypes = await client.GetAsync<string[]>(parameters);

            //Assert
            Assert.True(weatherTypes[3] == "Coooooooool");
        }

        [Fact]
        public async Task Test_Patch_Pass()
        {
            //Arrange

            //Get Client from dependency injection container
            var client = _serviceProvider.GetRequiredService<IApiGatewayClient>();

            JsonPatchDocument<WeatherForecast> jsonPatch = new JsonPatchDocument<WeatherForecast>();
            jsonPatch.Add(x => x.TemperatureC, 35);

            var parameters = new ApiGatewayParameters
            {
                ApiKey = "weatherservice",
                RouteKey = "patch",
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", "bearer wq298cjwosos==" }
                }
            };

            //Act
            var weatherForecast = await client.PatchAsync<WeatherForecast, WeatherForecast>(parameters, jsonPatch, true);

            //Assert
            Assert.True(weatherForecast.TemperatureC == 35);
        }

        [Fact]
        public async Task Test_Delete_Pass()
        {
            //Arrange

            //Get Client from dependency injection container
            var client = _serviceProvider.GetRequiredService<IApiGatewayClient>();

            var parameters = new ApiGatewayParameters
            {
                ApiKey = "weatherservice",
                RouteKey = "remove",
                Parameters = "0",
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", "bearer wq298cjwosos==" }
                }
            };

            //Act
            await client.DeleteAsync(parameters);

            parameters = new ApiGatewayParameters
            {
                ApiKey = "weatherservice",
                RouteKey = "types",
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", "bearer wq298cjwosos==" }
                }
            };

            var weatherTypes = await client.GetAsync<string[]>(parameters);

            //Assert
            Assert.DoesNotContain(weatherTypes, x => x == "Freezing");
        }

        //[Fact]
        //public async Task Test_GetOrchestration_Pass()
        //{
        //    //Arrange

        //    //Get Client from dependency injection container
        //    var client = _serviceProvider.GetRequiredService<IApiGatewayClient>();

        //    var parameters = new ApiGatewayParameters();

        //    //Act
        //    var orchestrations = await client.GetOrchestrationAsync(parameters, true);

        //    //Assert
        //    Assert.Equal(2, orchestrations.Count());
        //}
    }
}
