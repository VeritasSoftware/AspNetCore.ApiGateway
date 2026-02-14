using AspNetCore.ApiGateway.Minimal;
using System.Text;
using System.Text.Json;

namespace AspNetCore.ApiGateway.Application
{
    public static class MinimalAPIExtensions
    {
        public static RouteHandlerBuilder MapApiGatewayHead(this IEndpointRouteBuilder app)
        {
            return app.MapMethods("/api/Gateway/{apiKey}/{routeKey}", new[] { "HEAD" }, async (HttpRequest request, string apiKey, string routeKey, IApiGatewayRequestProcessor requestProcessor, string? parameters = null) =>
            {
                return Results.Ok(await requestProcessor.ProcessAsync(
                                        apiKey,
                                        routeKey,
                                        request,
                                        (client, apiInfo, routeInfo, content) => client.GetAsync($"{apiInfo.BaseUrl}{(routeInfo.IsParameterizedRoute ? routeInfo.GetPath(request) : routeInfo.Path + parameters)}"),
                                        null,
                                        null,
                                        parameters
                                     ));
            });
        }

        public static RouteHandlerBuilder MapApiGatewayGet(this IEndpointRouteBuilder app)
        {

            return app.MapGet("/api/Gateway/{apiKey}/{routeKey}", async (HttpRequest request, string apiKey, string routeKey, IApiGatewayRequestProcessor requestProcessor, string? parameters = null) =>
            {
                return Results.Ok(await requestProcessor.ProcessAsync(
                                        apiKey,
                                        routeKey,
                                        request,
                                        (client, apiInfo, routeInfo, content) => client.GetAsync($"{apiInfo.BaseUrl}{(routeInfo.IsParameterizedRoute ? routeInfo.GetPath(request) : routeInfo.Path + parameters)}"),
                                        null,
                                        null,
                                        parameters
                                     ));
            });
        }

        public static RouteHandlerBuilder MapApiGatewayPost(this IEndpointRouteBuilder app)
        {

            return app.MapPost("/api/Gateway/{apiKey}/{routeKey}", async (HttpRequest request, string apiKey, string routeKey, object requestObj, IApiGatewayRequestProcessor requestProcessor, string? parameters = null) =>
            {
                return Results.Ok(await requestProcessor.ProcessAsync(
                                        apiKey,
                                        routeKey,
                                        request,
                                        (client, apiInfo, routeInfo, content) => client.PostAsync($"{apiInfo.BaseUrl}{(routeInfo.IsParameterizedRoute ? routeInfo.GetPath(request) : routeInfo.Path + parameters)}", content),
                                        null,
                                        requestObj,
                                        parameters
                                     ));
            });
        }

        public static RouteHandlerBuilder MapApiGatewayPut(this IEndpointRouteBuilder app)
        {

            return app.MapPut("/api/Gateway/{apiKey}/{routeKey}", async (HttpRequest request, string apiKey, string routeKey, object requestObj, IApiGatewayRequestProcessor requestProcessor, string? parameters = null) =>
            {
                return Results.Ok(await requestProcessor.ProcessAsync(
                                        apiKey,
                                        routeKey,
                                        request,
                                        (client, apiInfo, routeInfo, content) => client.PutAsync($"{apiInfo.BaseUrl}{(routeInfo.IsParameterizedRoute ? routeInfo.GetPath(request) : routeInfo.Path + parameters)}", content),
                                        null,
                                        requestObj,
                                        parameters
                                     ));
            });
        }
        public static RouteHandlerBuilder MapApiGatewayPatch(this IEndpointRouteBuilder app)
        {

            return app.MapPatch("/api/Gateway/{apiKey}/{routeKey}", async (HttpRequest request, string apiKey, string routeKey, IApiGatewayRequestProcessor requestProcessor, string? parameters = null) =>
            {
                using var reader = new StreamReader(request.Body);

                string body = await reader.ReadToEndAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };

                var patch  = System.Text.Json.JsonSerializer.Deserialize<PatchObj>(body, options);                

                return Results.Ok(await requestProcessor.ProcessAsync(
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
            });
        }

        public static RouteHandlerBuilder MapApiGatewayDelete(this IEndpointRouteBuilder app)
        {

            return app.MapDelete("/api/Gateway/{apiKey}/{routeKey}", async (HttpRequest request, string apiKey, string routeKey, IApiGatewayRequestProcessor requestProcessor, string? parameters = null) =>
            {
                return Results.Ok(await requestProcessor.ProcessAsync(
                                        apiKey,
                                        routeKey,
                                        request,
                                        (client, apiInfo, routeInfo, content) => client.DeleteAsync($"{apiInfo.BaseUrl}{(routeInfo.IsParameterizedRoute ? routeInfo.GetPath(request) : routeInfo.Path + parameters)}"),
                                        null,
                                        null,
                                        parameters
                                     ));
            });
        }

        public static RouteHandlerBuilder MapApiGatewayGetOrchestration(this IEndpointRouteBuilder app)
        {

            return app.MapGet("/api/Gateway/orchestration", async (HttpRequest requesty, IApiOrchestrator apiOrchestrator, string? apiKey = null, string? routeKey = null) =>
            {
                apiKey = apiKey?.ToLower();
                routeKey = routeKey?.ToLower();

                var orchestrations = await Task.FromResult(string.IsNullOrEmpty(apiKey) && string.IsNullOrEmpty(routeKey)
                                                    ? apiOrchestrator.Orchestration
                                                    : (!string.IsNullOrEmpty(apiKey) && string.IsNullOrEmpty(routeKey)
                                                    ? apiOrchestrator.Orchestration?.Where(x => x.ApiKey.Contains(apiKey.Trim()))
                                                    : (string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(routeKey)
                                                    ? apiOrchestrator.Orchestration?.Where(x => x.ApiRoutes.Any(y => y.RouteKey.Contains(routeKey.Trim())))
                                                                                     .Select(x => x.FilterRoutes(routeKey))
                                                    : apiOrchestrator.Orchestration?.Where(x => x.ApiKey.Contains(apiKey.Trim()))
                                                                                     .Select(x => x.FilterRoutes(routeKey)))));

                return Results.Ok(orchestrations);
            });
        }
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