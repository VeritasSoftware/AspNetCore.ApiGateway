using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway
{
    internal class GatewayMiddleware
    {
        private readonly RequestDelegate _next;

        public GatewayMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var result = JsonSerializer.Serialize(new { error = ex.InnerException?.Message ?? ex.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(result);
        }

        public async Task Invoke(HttpContext context, IApiOrchestrator orchestrator, ILogger<ApiGatewayLog> logger)
        {
            ApiInfo apiInfo = null;
            HubInfo hubInfo = null;
            GatewayRouteInfo routeInfo = null;
            GatewayHubRouteInfo hubRouteInfo = null;

            try
            {
                var path = context.Request.Path.Value;

                var segmentsMatch = Regex.Match(path, GatewayConstants.GATEWAY_PATH_REGEX, RegexOptions.IgnoreCase | RegexOptions.Compiled);

                if (segmentsMatch.Success)
                {
                    var api = segmentsMatch.Groups["apiKey"].Captures[0].Value;
                    var key = segmentsMatch.Groups["routeKey"].Captures[0].Value;

                    if (path.ToLower().Contains("api/gateway/hub"))
                    {
                        hubInfo = orchestrator.GetHub(api);

                        hubRouteInfo = hubInfo.Mediator.GetRoute(key);
                    }
                    else
                    {
                        apiInfo = orchestrator.GetApi(api.ToString());

                        routeInfo = apiInfo.Mediator.GetRoute(key.ToString());
                    }                    

                    if ((routeInfo?.Verb.ToString() != context.Request.Method.ToUpper()) &&
                        (hubRouteInfo?.Verb.ToString() != context.Request.Method.ToUpper()))
                    {
                        throw new Exception("Invalid verb");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Api Gateway Orchestration api/key error.");

                context.Response.StatusCode = StatusCodes.Status404NotFound;

                await context.Response.CompleteAsync();

                return;
            }

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Api Gateway error.");

                await HandleExceptionAsync(context, ex);
            }
        }
    }
}
