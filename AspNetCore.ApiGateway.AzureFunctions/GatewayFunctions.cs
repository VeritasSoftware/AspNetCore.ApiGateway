using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace AspNetCore.ApiGateway.AzureFunctions
{
    public class GatewayFunctions
    {
        private readonly ILogger<GatewayFunctions> _logger;
        private readonly IApiGatewayRequestProcessor _requestProcessor;

        public GatewayFunctions(IApiGatewayRequestProcessor requestProcessor, ILogger<GatewayFunctions> logger = null)
        {
            _logger = logger;
            _requestProcessor = requestProcessor;
        }

        [Function("ApiGateway-Head")]
        public async Task<IActionResult> HeadAsync([HttpTrigger(AuthorizationLevel.Function, "head", Route = "api/Gateway/{apiKey}/{routeKey}")]
                                                            HttpRequest request, string apiKey, string routeKey, string parameters = null)
        {
            _logger?.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(await _requestProcessor.ProcessAsync(
                                        apiKey,
                                        routeKey,
                                        request,
                                        (client, apiInfo, routeInfo, content) => client.GetAsync($"{apiInfo.BaseUrl}{(routeInfo.IsParameterizedRoute ? routeInfo.GetPath(request) : routeInfo.Path + parameters)}"),
                                        null,
                                        null,
                                        parameters
                                     ));
        }

        [Function("ApiGateway-Get")]
        public async Task<IActionResult> GetAsync([HttpTrigger(AuthorizationLevel.Function, "get", Route = "api/Gateway/{apiKey}/{routeKey}")] 
                                                            HttpRequest request, string apiKey, string routeKey, string parameters = null)
        {
            _logger?.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(await _requestProcessor.ProcessAsync(
                                        apiKey,
                                        routeKey,
                                        request,
                                        (client, apiInfo, routeInfo, content) => client.GetAsync($"{apiInfo.BaseUrl}{(routeInfo.IsParameterizedRoute ? routeInfo.GetPath(request) : routeInfo.Path + parameters)}"),
                                        null,
                                        null,
                                        parameters
                                     ));
        }

        [Function("ApiGateway-Post")]
        public async Task<IActionResult> PostAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = "api/Gateway/{apiKey}/{routeKey}")]
                                                            HttpRequest request, string apiKey, string routeKey, string parameters = null)
        {
            _logger?.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            object requestObj = JsonConvert.DeserializeObject(requestBody);

            return new OkObjectResult(await _requestProcessor.ProcessAsync(
                                        apiKey,
                                        routeKey,
                                        request,
                                        (client, apiInfo, routeInfo, content) => client.PostAsync($"{apiInfo.BaseUrl}{(routeInfo.IsParameterizedRoute ? routeInfo.GetPath(request) : routeInfo.Path + parameters)}", content),
                                        null,
                                        requestObj,
                                        parameters
                                     ));
        }

        [Function("ApiGateway-Put")]
        public async Task<IActionResult> PutAsync([HttpTrigger(AuthorizationLevel.Function, "put", Route = "api/Gateway/{apiKey}/{routeKey}")]
                                                            HttpRequest request, string apiKey, string routeKey, string parameters = null)
        {
            _logger?.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            object requestObj = JsonConvert.DeserializeObject(requestBody);

            return new OkObjectResult(await _requestProcessor.ProcessAsync(
                                        apiKey,
                                        routeKey,
                                        request,
                                        (client, apiInfo, routeInfo, content) => client.PutAsync($"{apiInfo.BaseUrl}{(routeInfo.IsParameterizedRoute ? routeInfo.GetPath(request) : routeInfo.Path + parameters)}", content),
                                        null,
                                        requestObj,
                                        parameters
                                     ));
        }

        [Function("ApiGateway-Patch")]
        public async Task<IActionResult> PatchAsync([HttpTrigger(AuthorizationLevel.Function, "patch", Route = "api/Gateway/{apiKey}/{routeKey}")]
                                                            HttpRequest request, string apiKey, string routeKey, string parameters = null)
        {
            _logger?.LogInformation("C# HTTP trigger function processed a request.");

            using var reader = new StreamReader(request.Body);

            string body = await reader.ReadToEndAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            var patch = System.Text.Json.JsonSerializer.Deserialize<PatchObj>(body, options);

            return new OkObjectResult(await _requestProcessor.ProcessAsync(
                                    apiKey,
                                    routeKey,
                                    request,
                                    (client, apiInfo, routeInfo, content) => client.PatchAsync($"{apiInfo.BaseUrl}{(routeInfo.IsParameterizedRoute ? routeInfo.GetPath(request) : routeInfo.Path + parameters)}", content),
                                    request =>
                                    {
                                        var p = System.Text.Json.JsonSerializer.Serialize(request);

                                        return new StringContent(p, Encoding.UTF8, "application/json-patch+json");
                                    },
                                    patch.operations,
                                    parameters
                                 ));
        }

        [Function("ApiGateway-Delete")]
        public async Task<IActionResult> DeleteAsync([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "api/Gateway/{apiKey}/{routeKey}")]
                                                            HttpRequest request, string apiKey, string routeKey, string parameters = null)
        {
            _logger?.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(await _requestProcessor.ProcessAsync(
                                        apiKey,
                                        routeKey,
                                        request,
                                        (client, apiInfo, routeInfo, content) => client.DeleteAsync($"{apiInfo.BaseUrl}{(routeInfo.IsParameterizedRoute ? routeInfo.GetPath(request) : routeInfo.Path + parameters)}"),
                                        null,
                                        null,
                                        parameters
                                     ));
        }

        //[Function("ApiGateway-GetOrchestration")]
        //public async Task<IResult> GetOrchestrationAsync([HttpTrigger(AuthorizationLevel.Function, "get", Route = "/api/Gateway/orchestration")]
        //                                                    HttpRequest request, string apiKey = null, string routeKey = null)
        //{
        //    _logger?.LogInformation("C# HTTP trigger function processed a request.");

        //    var apiOrchestrator = _requestProcessor.ApiOrchestrator;

        //    apiKey = apiKey?.ToLower();
        //    routeKey = routeKey?.ToLower();

        //    var orchestrations = await Task.FromResult(string.IsNullOrEmpty(apiKey) && string.IsNullOrEmpty(routeKey)
        //                                        ? apiOrchestrator.Orchestration
        //                                        : (!string.IsNullOrEmpty(apiKey) && string.IsNullOrEmpty(routeKey)
        //                                        ? apiOrchestrator.Orchestration?.Where(x => x.ApiKey.Contains(apiKey.Trim()))
        //                                        : (string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(routeKey)
        //                                        ? apiOrchestrator.Orchestration?.Where(x => x.ApiRoutes.Any(y => y.RouteKey.Contains(routeKey.Trim())))
        //                                                                         .Select(x => x.FilterRoutes(routeKey))
        //                                        : apiOrchestrator.Orchestration?.Where(x => x.ApiKey.Contains(apiKey.Trim()))
        //                                                                         .Select(x => x.FilterRoutes(routeKey)))));

        //    return Results.Ok(orchestrations);
        //}
    }

    public class ContractResolver
    {
    }

    public class Operation
    {
        public int value { get; set; }
        public int operationType { get; set; }
        public string path { get; set; }
        public string op { get; set; }
        public object from { get; set; }
    }

    public class PatchObj
    {
        public List<Operation> operations { get; set; }
        public ContractResolver contractResolver { get; set; }
    }
}
