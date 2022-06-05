using AspNetCore.ApiGateway.Application.ActionFilters;
using AspNetCore.ApiGateway.Application.ExceptionFilters;
using AspNetCore.ApiGateway.Application.ResultFilters;
using AspNetCore.ApiGateway.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AspNetCore.ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(GatewayAuthorizeAttribute))]
    [ServiceFilter(typeof(GatewayAsyncActionFilterAttribute))]
    [ServiceFilter(typeof(GatewayAsyncExceptionFilterAttribute))]
    [ServiceFilter(typeof(GatewayAsyncResultFilterAttribute))]
    public class GatewayController : ControllerBase
    {
        readonly IApiOrchestrator _apiOrchestrator;
        readonly ILogger<ApiGatewayLog> _logger;
        readonly IHttpService _httpService;


        public GatewayController(IApiOrchestrator apiOrchestrator, ILogger<ApiGatewayLog> logger, IHttpService httpService)
        {
            _apiOrchestrator = apiOrchestrator;
            _logger = logger;
            _httpService = httpService;
        }

        [HttpGetOrHead]
        [Route("{api}/{key}")]
        [ServiceFilter(typeof(GatewayGetOrHeadAuthorizeAttribute))]
        [ServiceFilter(typeof(GatewayGetOrHeadAsyncActionFilterAttribute))]
        [ServiceFilter(typeof(GatewayGetOrHeadAsyncExceptionFilterAttribute))]
        [ServiceFilter(typeof(GatewayGetOrHeadAsyncResultFilterAttribute))]
        public async Task<IActionResult> Get(string api, string key, string parameters = null)
        {
            if (parameters != null)
                parameters = HttpUtility.UrlDecode(parameters);
            else
                parameters = string.Empty;

            _logger.LogApiInfo(api, key, parameters);

            var apiInfo = _apiOrchestrator.GetApi(api, true);

            var gwRouteInfo = apiInfo.Mediator.GetRoute(key);

            var routeInfo = gwRouteInfo.Route;

            if (routeInfo.Exec != null)
            {
                return Ok(await routeInfo.Exec(apiInfo, this.Request));
            }
            else
            {
                using (var client = routeInfo.HttpClientConfig?.HttpClient())
                {                    
                    this.Request.Headers?.AddRequestHeaders((client ?? _httpService.Client).DefaultRequestHeaders);

                    if (client == null)
                    {
                        routeInfo.HttpClientConfig?.CustomizeDefaultHttpClient?.Invoke(_httpService.Client, this.Request);
                    }

                    _logger.LogApiInfo($"{apiInfo.BaseUrl}{routeInfo.Path}{parameters}");

                    var response = await (client??_httpService.Client).GetAsync($"{apiInfo.BaseUrl}{(routeInfo.IsParameterizedRoute ? routeInfo.GetPath(this.Request) : routeInfo.Path + parameters)}");

                    response.EnsureSuccessStatusCode();

                    _logger.LogApiInfo($"{apiInfo.BaseUrl}{routeInfo.Path}{parameters}", false);

                    return Ok(routeInfo.ResponseType != null
                        ? JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), routeInfo.ResponseType)
                        : await response.Content.ReadAsStringAsync());
                }
            }
        }

        [HttpPost]
        [Route("{api}/{key}")]
        [ServiceFilter(typeof(GatewayPostAuthorizeAttribute))]
        [ServiceFilter(typeof(GatewayPostAsyncActionFilterAttribute))]
        [ServiceFilter(typeof(GatewayPostAsyncExceptionFilterAttribute))]
        [ServiceFilter(typeof(GatewayPostAsyncResultFilterAttribute))]
        public async Task<IActionResult> Post(string api, string key, object request, string parameters = null)
        {            
            if (parameters != null)
                parameters = HttpUtility.UrlDecode(parameters);
            else
                parameters = string.Empty;

            _logger.LogApiInfo(api, key, parameters, request);

            var apiInfo = _apiOrchestrator.GetApi(api, true);

            var gwRouteInfo = apiInfo.Mediator.GetRoute(key);

            var routeInfo = gwRouteInfo.Route;

            if (routeInfo.Exec != null)
            {
                return Ok(await routeInfo.Exec(apiInfo, this.Request));
            }
            else
            {
                using (var client = routeInfo.HttpClientConfig?.HttpClient())
                {                    
                    HttpContent content = null;

                    if (routeInfo.HttpClientConfig?.HttpContent != null)
                    {
                        content = routeInfo.HttpClientConfig.HttpContent();
                    }
                    else
                    {
                        content = new StringContent(request.ToString(), Encoding.UTF8, "application/json");

                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    }

                    this.Request.Headers?.AddRequestHeaders((client ?? _httpService.Client).DefaultRequestHeaders);

                    if (client == null)
                    {
                        routeInfo.HttpClientConfig?.CustomizeDefaultHttpClient?.Invoke(_httpService.Client, this.Request);
                    }

                    _logger.LogApiInfo($"{apiInfo.BaseUrl}{routeInfo.Path}{parameters}");

                    var response = await (client ?? _httpService.Client).PostAsync($"{apiInfo.BaseUrl}{(routeInfo.IsParameterizedRoute ? routeInfo.GetPath(this.Request) : routeInfo.Path + parameters)}", content);

                    _logger.LogApiInfo($"{apiInfo.BaseUrl}{routeInfo.Path}{parameters}", false);

                    response.EnsureSuccessStatusCode();

                    return Ok(routeInfo.ResponseType != null
                        ? JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), routeInfo.ResponseType)
                        : await response.Content.ReadAsStringAsync());
                }
            }
        }

        [HttpPost]
        [Route("hub/{api}/{key}")]
        [ServiceFilter(typeof(GatewayHubPostAuthorizeAttribute))]
        [ServiceFilter(typeof(GatewayHubPostAsyncActionFilterAttribute))]
        [ServiceFilter(typeof(GatewayHubPostAsyncExceptionFilterAttribute))]
        [ServiceFilter(typeof(GatewayHubPostAsyncResultFilterAttribute))]
        public async Task PostHub(string api, string key, params object[] request)
        {
            _logger.LogApiInfo(api, key, "", request);

            var hubInfo = _apiOrchestrator.GetHub(api);

            var gwRouteInfo = hubInfo.Mediator.GetRoute(key);           

            var connection = hubInfo.Connection;

            if (connection.State != HubConnectionState.Connected)
            {
                await connection.StartAsync();
            }

            await connection.InvokeCoreAsync(gwRouteInfo.HubRoute.InvokeMethod, request);
        }

        [HttpPut]
        [Route("{api}/{key}")]
        [ServiceFilter(typeof(GatewayPutAuthorizeAttribute))]
        [ServiceFilter(typeof(GatewayPutAsyncActionFilterAttribute))]
        [ServiceFilter(typeof(GatewayPutAsyncExceptionFilterAttribute))]
        [ServiceFilter(typeof(GatewayPutAsyncResultFilterAttribute))]
        public async Task<IActionResult> Put(string api, string key, object request, string parameters = null)
        {            
            if (parameters != null)
                parameters = HttpUtility.UrlDecode(parameters);
            else
                parameters = string.Empty;

            _logger.LogApiInfo(api, key, parameters, request);

            var apiInfo = _apiOrchestrator.GetApi(api, true);

            var gwRouteInfo = apiInfo.Mediator.GetRoute(key);

            var routeInfo = gwRouteInfo.Route;

            if (routeInfo.Exec != null)
            {
                return Ok(await routeInfo.Exec(apiInfo, this.Request));
            }
            else
            {
                using (var client = routeInfo.HttpClientConfig?.HttpClient())
                {                    
                    HttpContent content = null;

                    if (routeInfo.HttpClientConfig?.HttpContent != null)
                    {
                        content = routeInfo.HttpClientConfig.HttpContent();
                    }
                    else
                    {
                        content = new StringContent(request.ToString(), Encoding.UTF8, "application/json");

                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    }

                    this.Request.Headers?.AddRequestHeaders((client ?? _httpService.Client).DefaultRequestHeaders);

                    if (client == null)
                    {
                        routeInfo.HttpClientConfig?.CustomizeDefaultHttpClient?.Invoke(_httpService.Client, this.Request);
                    }

                    _logger.LogApiInfo($"{apiInfo.BaseUrl}{routeInfo.Path}{parameters}");

                    var response = await (client ?? _httpService.Client).PutAsync($"{apiInfo.BaseUrl}{(routeInfo.IsParameterizedRoute ? routeInfo.GetPath(this.Request) : routeInfo.Path + parameters)}", content);

                    _logger.LogApiInfo($"{apiInfo.BaseUrl}{routeInfo.Path}{parameters}", false);

                    response.EnsureSuccessStatusCode();

                    return Ok(routeInfo.ResponseType != null
                        ? JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), routeInfo.ResponseType)
                        : await response.Content.ReadAsStringAsync());
                }
            }
        }

        [HttpPatch]
        [Route("{api}/{key}")]
        [ServiceFilter(typeof(GatewayPatchAuthorizeAttribute))]
        [ServiceFilter(typeof(GatewayPatchAsyncActionFilterAttribute))]
        [ServiceFilter(typeof(GatewayPatchAsyncExceptionFilterAttribute))]
        [ServiceFilter(typeof(GatewayPatchAsyncResultFilterAttribute))]
        public async Task<IActionResult> Patch(string api, string key, [FromBody] JsonPatchDocument<object> patch, string parameters = null)
        {
            if (parameters != null)
                parameters = HttpUtility.UrlDecode(parameters);
            else
                parameters = string.Empty;

            _logger.LogApiInfo(api, key, parameters, patch.ToString());

            var apiInfo = _apiOrchestrator.GetApi(api, true);

            var gwRouteInfo = apiInfo.Mediator.GetRoute(key);

            var routeInfo = gwRouteInfo.Route;

            if (routeInfo.Exec != null)
            {
                return Ok(await routeInfo.Exec(apiInfo, this.Request));
            }
            else
            {
                using (var client = routeInfo.HttpClientConfig?.HttpClient())
                {                    
                    HttpContent content = null;

                    if (routeInfo.HttpClientConfig?.HttpContent != null)
                    {
                        content = routeInfo.HttpClientConfig.HttpContent();
                    }
                    else
                    {
                        var p = JsonConvert.SerializeObject(patch);

                        content = new StringContent(p, Encoding.UTF8, "application/json-patch+json");
                    }

                    this.Request.Headers?.AddRequestHeaders((client ?? _httpService.Client).DefaultRequestHeaders);

                    if (client == null)
                    {
                        routeInfo.HttpClientConfig?.CustomizeDefaultHttpClient?.Invoke(_httpService.Client, this.Request);
                    }

                    _logger.LogApiInfo($"{apiInfo.BaseUrl}{routeInfo.Path}{parameters}");

                    var response = await (client ?? _httpService.Client).PatchAsync($"{apiInfo.BaseUrl}{(routeInfo.IsParameterizedRoute ? routeInfo.GetPath(this.Request) : routeInfo.Path + parameters)}", content);

                    _logger.LogApiInfo($"{apiInfo.BaseUrl}{routeInfo.Path}{parameters}", false);

                    response.EnsureSuccessStatusCode();

                    return Ok(routeInfo.ResponseType != null
                        ? JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), routeInfo.ResponseType)
                        : await response.Content.ReadAsStringAsync());
                }
            }
        }

        [HttpDelete]
        [Route("{api}/{key}")]
        [ServiceFilter(typeof(GatewayDeleteAuthorizeAttribute))]
        [ServiceFilter(typeof(GatewayDeleteAsyncActionFilterAttribute))]
        [ServiceFilter(typeof(GatewayDeleteAsyncExceptionFilterAttribute))]
        [ServiceFilter(typeof(GatewayDeleteAsyncResultFilterAttribute))]
        public async Task<IActionResult> Delete(string api, string key, string parameters = null)
        {
            if (parameters != null)
            {
                parameters = HttpUtility.UrlDecode(parameters);
            }
            else
                parameters = string.Empty;

            _logger.LogApiInfo(api, key, parameters);

            var apiInfo = _apiOrchestrator.GetApi(api, true);

            var gwRouteInfo = apiInfo.Mediator.GetRoute(key);

            var routeInfo = gwRouteInfo.Route;

            if (routeInfo.Exec != null)
            {
                return Ok(await routeInfo.Exec(apiInfo, this.Request));
            }
            else
            {
                using (var client = routeInfo.HttpClientConfig?.HttpClient())
                {                    
                    this.Request.Headers?.AddRequestHeaders((client ?? _httpService.Client).DefaultRequestHeaders);

                    if (client == null)
                    {
                        routeInfo.HttpClientConfig?.CustomizeDefaultHttpClient?.Invoke(_httpService.Client, this.Request);
                    }

                    _logger.LogApiInfo($"{apiInfo.BaseUrl}{routeInfo.Path}{parameters}");

                    var response = await (client ?? _httpService.Client).DeleteAsync($"{apiInfo.BaseUrl}{(routeInfo.IsParameterizedRoute ? routeInfo.GetPath(this.Request) : routeInfo.Path + parameters)}");

                    _logger.LogApiInfo($"{apiInfo.BaseUrl}{routeInfo.Path}{parameters}", false);

                    response.EnsureSuccessStatusCode();

                    return Ok(routeInfo.ResponseType != null
                        ? JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), routeInfo.ResponseType)
                        : await response.Content.ReadAsStringAsync());
                }
            }
        }

        [HttpGet]
        [Route("orchestration")]
        [ServiceFilter(typeof(GatewayGetOrchestrationAuthorizeAttribute))]
        [ServiceFilter(typeof(GatewayGetOrchestrationAsyncActionFilterAttribute))]
        [ServiceFilter(typeof(GatewayGetOrchestrationAsyncExceptionFilterAttribute))]
        [ServiceFilter(typeof(GatewayGetOrchestrationAsyncResultFilterAttribute))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Orchestration>))]
        public async Task<IActionResult> GetOrchestration(string api = null, string key = null)
        {
            api = api?.ToLower();
            key = key?.ToLower();

            var orchestrations = await Task.FromResult(string.IsNullOrEmpty(api) && string.IsNullOrEmpty(key)
                                                ? _apiOrchestrator.Orchestration
                                                : (!string.IsNullOrEmpty(api) && string.IsNullOrEmpty(key)
                                                ? _apiOrchestrator.Orchestration?.Where(x => x.Api.Contains(api.Trim()))
                                                : (string.IsNullOrEmpty(api) && !string.IsNullOrEmpty(key)
                                                ? _apiOrchestrator.Orchestration?.Where(x => x.Routes.Any(y => y.Key.Contains(key.Trim())))
                                                                                 .Select(x => x.FilterRoutes(key))
                                                : _apiOrchestrator.Orchestration?.Where(x => x.Api.Contains(api.Trim()))
                                                                                 .Select(x => x.FilterRoutes(key)))));

            return Ok(orchestrations);
        }
    }
}