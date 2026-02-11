using AspNetCore.ApiGateway.Minimal;

namespace AspNetCore.ApiGateway.Application
{
    public static class MinimalAPIExtensions
    {

        public static RouteHandlerBuilder MapApiGatewayGet(this IEndpointRouteBuilder app)
        {

            return app.MapGet("/api/Gateway/{apiKey}/{routeKey}", async (HttpRequest request, string apiKey, string routeKey, IApiGatewayRequestProcessor requestProcessor, string parameters = null) =>
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

            return app.MapPost("/api/Gateway/{apiKey}/{routeKey}", async (HttpRequest request, string apiKey, string routeKey, object requestObj, IApiGatewayRequestProcessor requestProcessor, string parameters = null) =>
            {
                return Results.Ok(await requestProcessor.ProcessAsync(
                                        apiKey,
                                        routeKey,
                                        request,
                                        (client, apiInfo, routeInfo, content) => client.GetAsync($"{apiInfo.BaseUrl}{(routeInfo.IsParameterizedRoute ? routeInfo.GetPath(request) : routeInfo.Path + parameters)}"),
                                        null,
                                        requestObj,
                                        parameters
                                     ));
            });
        }
    }
}