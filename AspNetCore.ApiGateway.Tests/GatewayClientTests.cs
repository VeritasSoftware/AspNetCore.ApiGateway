using AspNetCore.ApiGateway.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
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

            var forecasts = await client.GetAsync<WeatherForecast[]>("weatherservice", "forecast");

            Assert.True(forecasts.Length > 0);
        }
    }
}
