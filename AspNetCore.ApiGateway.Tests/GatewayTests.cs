using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using GatewayAPI = ApiGateway.API;
using WeatherAPI = Weather.API;
using StockAPI = Stock.API;

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

            IWebHostBuilder stockAPI = new WebHostBuilder()
                                     .UseStartup<StockAPI.Startup>()
                                     .UseKestrel(options => options.Listen(IPAddress.Any, 63967));

            stockAPI.Start();

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
        public async Task Test_Get_Pass()
        {
            var client = _apiInit.GatewayAPI.CreateClient();

            //Gateway API url with Api key and Route key
            var gatewayUrl = "http://localhost/api/Gateway/weatherservice/forecast";

            var response = await client.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var forecasts = JsonConvert.DeserializeObject<WeatherAPI.WeatherForecast[]>(await response.Content.ReadAsStringAsync());

            Assert.True(forecasts.Length > 0);
        }

        [Fact]
        public async Task Test_Multiple_Get_Pass()
        {
            var client = _apiInit.GatewayAPI.CreateClient();

            //Gateway API url with Api key and Route key
            //Weather API call
            var gatewayUrl = "http://localhost/api/Gateway/weatherservice/forecast";

            var response = await client.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var forecasts = JsonConvert.DeserializeObject<WeatherAPI.WeatherForecast[]>(await response.Content.ReadAsStringAsync());

            Assert.True(forecasts.Length > 0);

            //Stock API call
            gatewayUrl = "http://localhost/api/Gateway/stockservice/stocks";

            response = await client.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var stockQuotes = JsonConvert.DeserializeObject<StockAPI.StockQuote[]>(await response.Content.ReadAsStringAsync());

            Assert.True(stockQuotes.Length > 0);
        }

        [Fact]
        public async Task Test_Get_WithParam_Pass()
        {
            var client = _apiInit.GatewayAPI.CreateClient();

            //Gateway API url with Api key, Route key and Param
            var gatewayUrl = "http://localhost/api/Gateway/weatherservice/type?parameters=3";

            var response = await client.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var weatherType = JsonConvert.DeserializeObject<WeatherAPI.WeatherTypeResponse>(await response.Content.ReadAsStringAsync());

            Assert.True(weatherType.Type == "Cool");
        }

        [Fact]
        public async Task Test_Get_Invalid_ApiKey_Fail()
        {
            var client = _apiInit.GatewayAPI.CreateClient();

            //Gateway API url with invalid Api key and Route key
            var gatewayUrl = "http://localhost/api/Gateway/xyzservice/forecast";

            var response = await client.GetAsync(gatewayUrl);

            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Test_Get_Invalid_RouteKey_Fail()
        {
            var client = _apiInit.GatewayAPI.CreateClient();

            //Gateway API url with Api key and invalid Route key
            var gatewayUrl = "http://localhost/api/Gateway/weatherservice/xyz";

            var response = await client.GetAsync(gatewayUrl);

            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
        }
    }
}
