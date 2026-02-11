using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;

namespace AspNetCore.ApiGateway.Minimal
{
    public interface IApiGatewayRequestProcessor
    {
        Task<object> ProcessAsync(string apiKey, string routeKey,
                                    HttpRequest httpRequest,
                                    Func<HttpClient, ApiInfo, RouteInfo, HttpContent, Task<HttpResponseMessage>> backEndCall,
                                    Func<object, StringContent> getContent = null,
                                    object request = null,
                                    string parameters = null);
    }

    public class ApiGatewayRequestProcessor : IApiGatewayRequestProcessor
    {
        readonly IApiOrchestrator _apiOrchestrator;
        readonly ILogger<ApiGatewayLog> _logger;
        readonly IHttpService _httpService;


        public ApiGatewayRequestProcessor(IApiOrchestrator apiOrchestrator, ILogger<ApiGatewayLog> logger, IHttpService httpService)
        {
            _apiOrchestrator = apiOrchestrator;
            _logger = logger;
            _httpService = httpService;
        }

        public async Task<object> ProcessAsync(
                string apiKey,
                string routeKey,
                HttpRequest httpRequest,
                Func<HttpClient, ApiInfo, RouteInfo, HttpContent, Task<HttpResponseMessage>> backEndCall,
                Func<object, StringContent> getContent = null,
                object request = null,
                string parameters = null)
        {
            if (parameters != null)
                parameters = HttpUtility.UrlDecode(parameters);
            else
                parameters = string.Empty;

            _logger.LogApiInfo(apiKey, routeKey, parameters, request);

            var apiInfo = _apiOrchestrator.GetApi(apiKey, true);

            var gwRouteInfo = apiInfo.Mediator.GetRoute(routeKey);

            var routeInfo = gwRouteInfo.Route;

            if (routeInfo.Exec != null)
            {
                return await routeInfo.Exec(apiInfo, httpRequest);
            }
            else
            {
                using (var client = routeInfo.HttpClientConfig?.HttpClient())
                {
                    HttpContent content = null;

                    if (request != null)
                    {
                        if (routeInfo.HttpClientConfig?.HttpContent != null)
                        {
                            content = routeInfo.HttpClientConfig.HttpContent();
                        }
                        else
                        {
                            if (getContent != null)
                            {
                                content = getContent(request);
                            }
                            else
                            {
                                content = new StringContent(request.ToString(), Encoding.UTF8, "application/json");

                                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            }
                        }
                    }

                    httpRequest.Headers?.AddRequestHeaders((client ?? _httpService.Client).DefaultRequestHeaders);

                    if (client == null)
                    {
                        routeInfo.HttpClientConfig?.CustomizeDefaultHttpClient?.Invoke(_httpService.Client, httpRequest);
                    }

                    _logger.LogApiInfo($"{apiInfo.BaseUrl}{routeInfo.Path}{parameters}");

                    var response = await backEndCall((client ?? _httpService.Client), apiInfo, routeInfo, content);

                    _logger.LogApiInfo($"{apiInfo.BaseUrl}{routeInfo.Path}{parameters}", false);

                    response.EnsureSuccessStatusCode();

                    var returnedContent = await response.Content.ReadAsStringAsync();

                    return await Task.FromResult(routeInfo.ResponseType != null
                        ? !string.IsNullOrEmpty(returnedContent) ? JsonSerializer.Deserialize(returnedContent, routeInfo.ResponseType) : string.Empty
                        : returnedContent);
                }
            }
        }
    }
}
