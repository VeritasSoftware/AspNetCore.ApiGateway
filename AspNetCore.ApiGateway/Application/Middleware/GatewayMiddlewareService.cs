using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Middleware
{
    internal class GatewayMiddlewareService
    {
        private readonly RequestDelegate _next;

        public GatewayMiddlewareService(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<ApiGatewayLog> logger, IGatewayMiddleware gatewayMiddleware)
        {
            try
            {
                var path = context.Request.Path.Value;

                var segmentsMatch = Regex.Match(path, GatewayConstants.GATEWAY_PATH_REGEX, RegexOptions.IgnoreCase | RegexOptions.Compiled);

                if (segmentsMatch.Success)
                {
                    var api = segmentsMatch.Groups["apiKey"].Captures[0].Value;
                    var key = segmentsMatch.Groups["routeKey"].Captures[0].Value;

                    await gatewayMiddleware.Invoke(context, api.ToString(), key.ToString());
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Api Gateway middleware service error.");

                throw ex;
            }

            await _next(context);
        }
    }
}
