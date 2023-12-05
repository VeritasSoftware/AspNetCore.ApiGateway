using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using GatewayAPI = ApiGateway.API;
using WeatherAPI = Weather.API;
using StockAPI = Stock.API;
using System.Net.Http;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using System.Text.Json;

namespace AspNetCore.ApiGateway.Tests
{
    public class APIInitialize
    {
        public TestServer GatewayAPI { get; set; }

        public APIInitialize()
        {
            //Start Weather API
            IWebHostBuilder weatherAPI = new WebHostBuilder()
                                     .UseStartup<WeatherAPI.Startup>()
                                     .UseKestrel(options => options.Listen(IPAddress.Any, 5003, listenOptions => listenOptions.UseHttps(o => o.AllowAnyClientCertificate())));

            weatherAPI.Start();

            //Start Stock API
            IWebHostBuilder stockAPI = new WebHostBuilder()
                                     .UseStartup<StockAPI.Startup>()
                                     .UseKestrel(options => options.Listen(IPAddress.Any, 5005, listenOptions => listenOptions.UseHttps(o => o.AllowAnyClientCertificate())));

            stockAPI.Start();

            //Start Gateway API
            IWebHostBuilder gatewayAPI = new WebHostBuilder()
                                     .UseStartup<GatewayAPI.Startup>()
                                     .UseKestrel(options => options.Listen(IPAddress.Any, 5001, listenOptions => listenOptions.UseHttps(o => o.AllowAnyClientCertificate())));

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
            var gatewayUrl = "https://localhost:5001/api/Gateway/weatherservice/forecast";

            var response = await client.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var forecasts = JsonSerializer.Deserialize<WeatherForecast[]>(await response.Content.ReadAsStringAsync());

            Assert.True(forecasts.Length > 0);
        }

        [Fact]
        public async Task Test_Multiple_Get_Pass()
        {
            var client = _apiInit.GatewayAPI.CreateClient();

            //Gateway API url with Api key and Route key
            //Weather API call
            var gatewayUrl = "https://localhost:5001/api/Gateway/weatherservice/forecast";

            var response = await client.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var forecasts = JsonSerializer.Deserialize<WeatherForecast[]>(await response.Content.ReadAsStringAsync());

            Assert.True(forecasts.Length > 0);

            client = _apiInit.GatewayAPI.CreateClient();

            //Stock API call
            gatewayUrl = "https://localhost:5001/api/Gateway/stockservice/stocks";

            response = await client.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var stockQuotes = JsonSerializer.Deserialize<StockQuote[]>(await response.Content.ReadAsStringAsync());

            Assert.True(stockQuotes.Length > 0);
        }

        [Fact]
        public async Task Test_Get_WithParam_Pass()
        {
            var client = _apiInit.GatewayAPI.CreateClient();

            //Gateway API url with Api key, Route key and Param
            var gatewayUrl = "https://localhost:5001/api/Gateway/weatherservice/type?parameters=3";

            var response = await client.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var weatherType = JsonSerializer.Deserialize<WeatherTypeResponse>(await response.Content.ReadAsStringAsync());

            Assert.NotNull(weatherType);
            Assert.True(!string.IsNullOrEmpty(weatherType.Type));

            gatewayUrl = "https://localhost:5001/api/Gateway/weatherservice/typewithparams?parameters=index=3";

            response = await client.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            weatherType = JsonSerializer.Deserialize<WeatherTypeResponse>(await response.Content.ReadAsStringAsync());

            Assert.NotNull(weatherType);
            Assert.True(!string.IsNullOrEmpty(weatherType.Type));
        }

        [Fact]
        public async Task Test_Post_Pass()
        {
            var client = _apiInit.GatewayAPI.CreateClient();

            //Gateway API url with Api key and Route key
            var gatewayUrl = "https://localhost:5001/api/Gateway/weatherservice/add";

            AddWeatherTypeRequest request = new AddWeatherTypeRequest
            {
                WeatherType = "Windy"
            };

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var httprequest = new HttpRequestMessage
            {
                RequestUri = new Uri(gatewayUrl),
                Content = content,
                Method = HttpMethod.Post
            };

            var response = await client.SendAsync(httprequest);

            response.EnsureSuccessStatusCode();

            var weatherTypes = JsonSerializer.Deserialize<string[]>(await response.Content.ReadAsStringAsync());

            Assert.True(weatherTypes.Last() == "Windy");
        }

        [Fact]
        public async Task Test_Put_Pass()
        {
            var client = _apiInit.GatewayAPI.CreateClient();

            //Gateway API url with Api key and Route key
            var gatewayUrl = "https://localhost:5001/api/Gateway/weatherservice/update";

            UpdateWeatherTypeRequest request = new UpdateWeatherTypeRequest
            {
                WeatherType = "Coooooooool",
                Index = 3
            };

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var httprequest = new HttpRequestMessage
            {
                RequestUri = new Uri(gatewayUrl),
                Content = content,
                Method = HttpMethod.Put
            };

            var response = await client.SendAsync(httprequest);

            response.EnsureSuccessStatusCode();

            //Gateway API url with Api key and Route key
            gatewayUrl = "https://localhost:5001/api/Gateway/weatherservice/types";

            response = await client.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var weatherTypes = JsonSerializer.Deserialize<string[]>(await response.Content.ReadAsStringAsync());

            Assert.True(weatherTypes[3] == "Coooooooool");
        }        

        [Fact]
        public async Task Test_Patch_Pass()
        {
            var client = _apiInit.GatewayAPI.CreateClient();

            //Gateway API url with Api key and Route key
            var gatewayUrl = "https://localhost:5001/api/Gateway/weatherservice/patch";            

            JsonPatchDocument<WeatherForecast> jsonPatch = new JsonPatchDocument<WeatherForecast>();
            jsonPatch.Add(x => x.TemperatureC, 35);

            var jsonContent = JsonSerializer.Serialize(jsonPatch.Operations);

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json-patch+json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json-patch+json");

            var httprequest = new HttpRequestMessage
            {
                RequestUri = new Uri(gatewayUrl),
                Content = content,
                Method = HttpMethod.Patch
            };

            var response = await client.SendAsync(httprequest);

            response.EnsureSuccessStatusCode();

            var weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(await response.Content.ReadAsStringAsync());

            Assert.True(weatherForecast.TemperatureC == 35);
        }

        [Fact]
        public async Task Test_Delete_Pass()
        {
            var client = _apiInit.GatewayAPI.CreateClient();

            //Gateway API url with Api key, Route key and Param
            var gatewayUrl = "https://localhost:5001/api/Gateway/weatherservice/remove?parameters=0";

            var response = await client.DeleteAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            //Gateway API url with Api key and Route key
            gatewayUrl = "https://localhost:5001/api/Gateway/weatherservice/types";

            response = await client.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var weatherTypes = JsonSerializer.Deserialize<string[]>(await response.Content.ReadAsStringAsync());

            Assert.DoesNotContain(weatherTypes, x => x == "Freezing");
        }

        [Fact]
        public async Task Test_Get_Invalid_ApiKey_Fail()
        {
            var client = _apiInit.GatewayAPI.CreateClient();

            //Gateway API url with invalid Api key and Route key
            var gatewayUrl = "https://localhost:5001/api/Gateway/xyzservice/forecast";

            var response = await client.GetAsync(gatewayUrl);

            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Test_Get_Invalid_RouteKey_Fail()
        {
            var client = _apiInit.GatewayAPI.CreateClient();

            //Gateway API url with Api key and invalid Route key
            var gatewayUrl = "https://localhost:5001/api/Gateway/weatherservice/xyz";

            var response = await client.GetAsync(gatewayUrl);

            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Test_GetOrchestration_Pass()
        {
            var client = _apiInit.GatewayAPI.CreateClient();

            //Gateway API Orchestration url
            var gatewayUrl = "https://localhost:5001/api/Gateway/orchestration";

            var response = await client.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var orchestration = JsonSerializer.Deserialize<Orchestration[]>(await response.Content.ReadAsStringAsync());

            Assert.True(orchestration.Length > 0);
        }
    }
}
