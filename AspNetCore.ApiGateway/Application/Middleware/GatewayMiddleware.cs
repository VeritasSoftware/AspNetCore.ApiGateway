using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.RegularExpressions;

namespace AspNetCore.ApiGateway
{
    internal static class GatewayMiddlewareExtensions
    {
        public static void UseGatewayMiddleware(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                ApiInfo apiInfo = null;
                GatewayRouteInfo routeInfo = null;

                try
                {
                    var path = context.Request.Path.Value;

                    var segmentsMatch = Regex.Match(path, "^/?api/Gateway(/(?<api>.*?)/(?<key>.*?)(/.*?)?)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                    if (segmentsMatch.Success)
                    {
                        var api = segmentsMatch.Groups["api"].Captures[0].Value;
                        var key = segmentsMatch.Groups["key"].Captures[0].Value;

                        apiInfo = app.ApplicationServices.GetRequiredService<IApiOrchestrator>().GetApi(api.ToString());

                        routeInfo = apiInfo.Mediator.GetRoute(key.ToString());

                        if (routeInfo.Verb.ToString() != context.Request.Method.ToUpper())
                        {
                            throw new Exception("Invalid verb");
                        }
                    }

                    await next();
                }
                catch (Exception)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;

                    await context.Response.CompleteAsync();
                }
            });
        }
    }
}
