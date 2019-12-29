using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using GatewayAPI = ApiGateway.API;
using WeatherAPI = Weather.API;

namespace AspNetCore.ApiGateway.Tests
{
    public class APIInitialize
    {
        public TestServer GatewayAPI { get; set; }

        public APIInitialize()
        {
            IWebHostBuilder weatherAPI = new WebHostBuilder()
                                     .UseStartup<WeatherAPI.Startup>()
                                     .UseKestrel(options => options.Listen(IPAddress.Any, 63969));

            weatherAPI.Start();

            IWebHostBuilder gatewayAPI = new WebHostBuilder()
                                     .UseStartup<GatewayAPI.Startup>()
                                     .UseKestrel(options => options.Listen(IPAddress.Any, 80));

            TestServer gatewayServer = new TestServer(gatewayAPI);

            this.GatewayAPI = gatewayServer;
        }
    }

    public class GatewayTests : IClassFixture<APIInitialize>
    {
        readonly APIInitialize _apiInit;

        public GatewayTests(APIInitialize apiInit)
        {
            _apiInit = apiInit;
        }

        [Fact]
        public async Task Test_Get()
        {
            var client = _apiInit.GatewayAPI.CreateClient();

            var response = await client.GetAsync("http://localhost/api/Gateway/weatherservice/forecast");

            response.EnsureSuccessStatusCode();

            var forecasts = JsonConvert.DeserializeObject<WeatherAPI.WeatherForecast[]>(await response.Content.ReadAsStringAsync());

            Assert.True(forecasts.Length > 0);
        }

        [Fact]
        public async Task Test_Get_Invalid_ApiKey_Fail()
        {
            var client = _apiInit.GatewayAPI.CreateClient();

            var response = await client.GetAsync("http://localhost/api/Gateway/xyzservice/forecast");

            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Test_Get_Invalid_RouteKey_Fail()
        {
            var client = _apiInit.GatewayAPI.CreateClient();

            var response = await client.GetAsync("http://localhost/api/Gateway/weatherservice/xyz");

            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
        }
    }
}
