using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

                var segmentsMatch = Regex.Match(path, "^/?api/Gateway(/(?!orchestration)(hub/)?(?<api>.*?)/(?<key>.*?)(/.*?)?)?$",
                                                    RegexOptions.IgnoreCase | RegexOptions.Compiled);

                if (segmentsMatch.Success)
                {
                    var api = segmentsMatch.Groups["api"].Captures[0].Value;
                    var key = segmentsMatch.Groups["key"].Captures[0].Value;

                    await gatewayMiddleware.Invoke(context, api.ToString(), key.ToString());
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Api Gateway middleware service error.");

                context.Response.StatusCode = StatusCodes.Status404NotFound;

                await context.Response.CompleteAsync();

                return;
            }

            await _next(context);
        }
    }
}
