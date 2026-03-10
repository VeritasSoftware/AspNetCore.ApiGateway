using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AspNetCore.ApiGateway.AzureFunctions
{
    public class GatewayMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly IApiOrchestrator _orchestrator;
        private readonly ILogger<ApiGatewayLog> _logger;

        public GatewayMiddleware(IApiOrchestrator orchestrator, ILogger<ApiGatewayLog> logger)
        {
            _orchestrator = orchestrator;
            _logger = logger;
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var result = JsonSerializer.Serialize(new { error = ex.InnerException?.Message ?? ex.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(result);
        }        

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            ApiInfo apiInfo = null;
            GatewayRouteInfo routeInfo = null;

            var httpContext = context.GetHttpContext();

            try
            {
                var path = httpContext.Request.Path.Value;

                var segmentsMatch = Regex.Match(path, GatewayConstants.GATEWAY_PATH_REGEX, RegexOptions.IgnoreCase | RegexOptions.Compiled);

                if (segmentsMatch.Success)
                {
                    var api = segmentsMatch.Groups["apiKey"].Captures[0].Value;
                    var key = segmentsMatch.Groups["routeKey"].Captures[0].Value;

                    apiInfo = _orchestrator.GetApi(api.ToString());

                    routeInfo = apiInfo.Mediator.GetRoute(key.ToString());

                    if ((routeInfo?.Verb.ToString() != httpContext.Request.Method.ToUpper()))
                    {
                        throw new Exception("Invalid verb");
                    }

                    if (routeInfo.Route.ResponseCachingDurationInSeconds  > 0)
                    {
                        var response = httpContext.Response;

                        response.Headers.Append("Cache-Control", $"public, max-age={routeInfo.Route.ResponseCachingDurationInSeconds * 1000}");
                        response.Headers.Append("Pragma", "cache");
                        response.Headers.Append("Expires", DateTime.UtcNow.AddSeconds(routeInfo.Route.ResponseCachingDurationInSeconds).ToString("R"));

                        response.Headers.Append("X-Cache-Info", $"Cached for {routeInfo.Route.ResponseCachingDurationInSeconds} seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Api Gateway Orchestration api/key error.");

                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

                await httpContext.Response.CompleteAsync();

                return;
            }

            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Api Gateway error.");

                await HandleExceptionAsync(httpContext, ex);
            }
        }
    }
}
