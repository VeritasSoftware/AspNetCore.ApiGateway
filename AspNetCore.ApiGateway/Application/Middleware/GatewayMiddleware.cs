using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway
{
    internal static class GatewayMiddlewareExtensions
    {
        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected            

            var result = JsonConvert.SerializeObject(new { error = ex.InnerException.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            await context.Response.WriteAsync(result);
        }

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
                }
                catch (Exception)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;

                    await context.Response.CompleteAsync();

                    return;
                }

                try
                {
                    await next();
                }
                catch(Exception ex)
                {
                    await HandleExceptionAsync(context, ex);
                }
            });
        }
    }
}
