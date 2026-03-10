using AspNetCore.ApiGateway.Tests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Learn.AzureFunctionsTesting;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using StockAPI = Stock.API;
using WeatherAPI = Weather.API;

[assembly: TestFramework("Microsoft.Learn.AzureFunctionsTesting.TestFramework", "Microsoft.Learn.AzureFunctionsTesting")]
[assembly: AssemblyFixture(typeof(FunctionFixture<FunctionStartup>))]
namespace AspNetCore.ApiGateway.Tests
{
    public class FunctionStartup : IFunctionTestStartup
    {
        public void Configure(FunctionTestConfigurationBuilder builder)
        {
            var path = Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\Sample.ApiGateway.AzureFunctions\bin\Debug\net8.0");
            builder.SetFunctionAppPath(path);
            builder.SetFunctionAppPort(7055);
        }
    }

    // Define a collection for the FunctionFixture<FunctionStartup>
    [CollectionDefinition("Function collection")]
    public class FunctionCollection : ICollectionFixture<FunctionFixture<FunctionStartup>>
    {
        // This class has no code, and is never created. Its purpose is just to be the place to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
    }

    public class AzureFunctionsAPIInitialize
    {
        public TestServer GatewayAPI { get; set; }

        public AzureFunctionsAPIInitialize()
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
        }
    }

    [Collection("Function collection")]
    public class AzureFunctionsGatewayTests : IClassFixture<AzureFunctionsAPIInitialize>
    {
        //readonly AzureFunctionsAPIInitialize _apiInit;
        private readonly HttpClient _httpClient;

        public AzureFunctionsGatewayTests(AzureFunctionsAPIInitialize apiInit, FunctionFixture<FunctionStartup> fixture)
        {
            //_apiInit = apiInit;
            _httpClient = fixture.Client;
            //_httpClient.BaseAddress = new Uri("https://localhost:7055");
        }

        [Fact]
        public async Task Test_Get_Pass()
        {
            var client = _httpClient;

            //Gateway API url with Api key and Route key
            var gatewayUrl = "api/Gateway/weatherservice/forecast";

            var response = await client.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var forecasts = JsonSerializer.Deserialize<WeatherForecast[]>(await response.Content.ReadAsStringAsync());

            Assert.True(forecasts.Length > 0);
        }

        [Fact]
        public async Task Test_Multiple_Get_Pass()
        {
            var client = _httpClient;

            //Gateway API url with Api key and Route key
            //Weather API call
            var gatewayUrl = "api/Gateway/weatherservice/forecast";

            var response = await client.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var forecasts = JsonSerializer.Deserialize<WeatherForecast[]>(await response.Content.ReadAsStringAsync());

            Assert.True(forecasts.Length > 0);

            client = _httpClient;

            //Stock API call
            gatewayUrl = "api/Gateway/stockservice/stocks";

            response = await client.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var stockQuotes = JsonSerializer.Deserialize<StockQuote[]>(await response.Content.ReadAsStringAsync());

            Assert.True(stockQuotes.Length > 0);
        }

        [Fact]
        public async Task Test_Get_WithParam_Pass()
        {
            var client = _httpClient;

            //Gateway API url with Api key, Route key and Param
            var gatewayUrl = "api/Gateway/weatherservice/type?parameters=3";

            var response = await client.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var strResponse = await response.Content.ReadAsStringAsync();

            var weatherType = JsonSerializer.Deserialize<WeatherTypeResponse>(strResponse);

            Assert.NotNull(weatherType);
            Assert.True(!string.IsNullOrEmpty(weatherType.Type));

            //client = _httpClient;

            //gatewayUrl = "api/Gateway/weatherservice/typewithparams?parameters=index=3";

            //response = await client.GetAsync(gatewayUrl);

            //response.EnsureSuccessStatusCode();

            //strResponse = await response.Content.ReadAsStringAsync();

            //weatherType = JsonSerializer.Deserialize<WeatherTypeResponse>(strResponse);

            //Assert.NotNull(weatherType);
            //Assert.True(!string.IsNullOrEmpty(weatherType.Type));
        }

        [Fact]
        public async Task Test_Post_Pass()
        {
            var client = _httpClient;

            //Gateway API url with Api key and Route key
            var gatewayUrl = $"api/Gateway/weatherservice/add";

            AddWeatherTypeRequest request = new AddWeatherTypeRequest
            {
                WeatherType = "Windy"
            };

            // POST JSON
            HttpResponseMessage response = await client.PostAsJsonAsync(gatewayUrl, request);

            //var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //var httprequest = new HttpRequestMessage
            //{
            //    //RequestUri = new Uri($"{client.BaseAddress}{gatewayUrl}"),
            //    RequestUri = new Uri(gatewayUrl),
            //    Content = content,
            //    Method = HttpMethod.Post
            //};

            //var response = await client.SendAsync(httprequest);

            response.EnsureSuccessStatusCode();

            var weatherTypes = JsonSerializer.Deserialize<string[]>(await response.Content.ReadAsStringAsync());

            Assert.True(weatherTypes.Last() == "Windy");
        }

        [Fact]
        public async Task Test_Put_Pass()
        {
            var client = _httpClient;

            //Gateway API url with Api key and Route key
            var gatewayUrl = "api/Gateway/weatherservice/update";

            UpdateWeatherTypeRequest request = new UpdateWeatherTypeRequest
            {
                WeatherType = "Coooooooool",
                Index = 3
            };

            // POST JSON
            HttpResponseMessage response = await client.PutAsJsonAsync(gatewayUrl, request);

            //var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //var httprequest = new HttpRequestMessage
            //{
            //    RequestUri = new Uri(gatewayUrl),
            //    Content = content,
            //    Method = HttpMethod.Put
            //};

            //var response = await client.SendAsync(httprequest);

            response.EnsureSuccessStatusCode();

            //Gateway API url with Api key and Route key
            gatewayUrl = "api/Gateway/weatherservice/types";

            response = await client.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var weatherTypes = JsonSerializer.Deserialize<string[]>(await response.Content.ReadAsStringAsync());

            Assert.True(weatherTypes[3] == "Coooooooool");
        }        

        [Fact]
        public async Task Test_Patch_Pass()
        {
            var client = _httpClient;

            //Gateway API url with Api key and Route key
            var gatewayUrl = "api/Gateway/weatherservice/patch";            

            JsonPatchDocument<WeatherForecast> jsonPatch = new JsonPatchDocument<WeatherForecast>();
            jsonPatch.Add(x => x.TemperatureC, 35);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var response = await client.PatchAsJsonAsync(gatewayUrl, jsonPatch, options);

            response.EnsureSuccessStatusCode();

            var strResponse = await response.Content.ReadAsStringAsync();

            var weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(strResponse);

            Assert.True(weatherForecast.TemperatureC == 35);
        }

        [Fact]
        public async Task Test_Delete_Pass()
        {
            var client = _httpClient;

            //Gateway API url with Api key, Route key and Param
            var gatewayUrl = "api/Gateway/weatherservice/remove?parameters=0";

            var response = await client.DeleteAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            //Gateway API url with Api key and Route key
            gatewayUrl = "api/Gateway/weatherservice/types";

            response = await client.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var weatherTypes = JsonSerializer.Deserialize<string[]>(await response.Content.ReadAsStringAsync());

            Assert.DoesNotContain(weatherTypes, x => x == "Freezing");
        }

        //[Fact]
        //public async Task Test_Get_Invalid_ApiKey_Fail()
        //{
        //    var client = _httpClient;

        //    //Gateway API url with invalid Api key and Route key
        //    var gatewayUrl = "api/Gateway/xyzservice/forecast";

        //    var response = await client.GetAsync(gatewayUrl);

        //    Assert.True(response.StatusCode == HttpStatusCode.NotFound);
        //}

        //[Fact]
        //public async Task Test_Get_Invalid_RouteKey_Fail()
        //{
        //    var client = _httpClient;

        //    //Gateway API url with Api key and invalid Route key
        //    var gatewayUrl = "api/Gateway/weatherservice/xyz";

        //    var response = await client.GetAsync(gatewayUrl);

        //    Assert.True(response.StatusCode == HttpStatusCode.NotFound);
        //}

        //[Fact]
        //public async Task Test_GetOrchestration_Pass()
        //{
        //    var client = _httpClient;

        //    //Gateway API Orchestration url
        //    var gatewayUrl = "api/Gateway/orchestration";

        //    var response = await client.GetAsync(gatewayUrl);

        //    response.EnsureSuccessStatusCode();

        //    var strResponse = await response.Content.ReadAsStringAsync();

        //    var orchestration = JsonSerializer.Deserialize<Orchestration[]>(strResponse);

        //    Assert.True(orchestration.Length > 0);
        //}
    }
}
