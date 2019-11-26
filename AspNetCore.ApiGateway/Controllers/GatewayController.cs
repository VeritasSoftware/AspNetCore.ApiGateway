using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AspNetCore.ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GatewayController : ControllerBase
    {
        readonly IApiOrchestrator _apiOrchestrator;

        public GatewayController(IApiOrchestrator apiOrchestrator)
        {
            _apiOrchestrator = apiOrchestrator;
        }

        [HttpGet]
        [Route("{api}/{key}")]
        public async Task<IActionResult> GetNoParams(string api, string key)
        {
            return await this.Get(api, key);            
        }

        [HttpGet]
        [Route("{api}/{key}/{parameters}")]
        public async Task<IActionResult> GetParams(string api, string key, string parameters)
        {
            return await this.Get(api, key, parameters);
        }

        [HttpPost]
        [Route("{api}/{key}")]
        public async Task<IActionResult> Post(string api, string key, object request)
        {
            var apiInfo = _apiOrchestrator.GetApi(api);

            var routeInfo = apiInfo.Mediator.GetRoute(key);
            
            if (routeInfo.Exec != null)
            {
                return Ok(await routeInfo.Exec(apiInfo, routeInfo, this.Request));
            }
            else
            {
                using (var client = routeInfo.HttpClientConfig?.HttpClient() ?? new HttpClient())
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

                    this.Request.Headers?.AddRequestHeaders(client.DefaultRequestHeaders);
                    
                    var response = await client.PostAsync($"{apiInfo.BaseUrl}{routeInfo.Path}", content);

                    response.EnsureSuccessStatusCode();

                    return Ok(JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), routeInfo.ResponseType));
                }
            }
        }

        [HttpPut]
        [Route("{api}/{key}")]
        public async Task<IActionResult> Put(string api, string key, object request)
        {
            var apiInfo = _apiOrchestrator.GetApi(api);

            var routeInfo = apiInfo.Mediator.GetRoute(key);

            if (routeInfo.Exec != null)
            {
                return Ok(await routeInfo.Exec(apiInfo, routeInfo, this.Request));
            }
            else
            {
                using (var client = routeInfo.HttpClientConfig?.HttpClient() ?? new HttpClient())
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

                    this.Request.Headers?.AddRequestHeaders(client.DefaultRequestHeaders);

                    var response = await client.PutAsync($"{apiInfo.BaseUrl}{routeInfo.Path}", content);

                    response.EnsureSuccessStatusCode();

                    return Ok(JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), routeInfo.ResponseType));
                }
            }
        }

        [HttpDelete]
        [Route("{api}/{key}/{parameters}")]
        public async Task<IActionResult> Delete(string api, string key, string parameters)
        {
            parameters = HttpUtility.UrlDecode(parameters);

            var apiInfo = _apiOrchestrator.GetApi(api);

            var routeInfo = apiInfo.Mediator.GetRoute(key);

            if (routeInfo.Exec != null)
            {
                return Ok(await routeInfo.Exec(apiInfo, routeInfo, this.Request));
            }
            else
            {
                using (var client = routeInfo.HttpClientConfig?.HttpClient() ?? new HttpClient())
                {
                    this.Request.Headers?.AddRequestHeaders(client.DefaultRequestHeaders);

                    var response = await client.DeleteAsync($"{apiInfo.BaseUrl}{routeInfo.Path}{parameters}");

                    response.EnsureSuccessStatusCode();

                    return Ok(JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), routeInfo.ResponseType));
                }
            }
        }

        private async Task<IActionResult> Get(string apiKey, string key, string parameters = "")
        {
            parameters = HttpUtility.UrlDecode(parameters);

            var apiInfo = _apiOrchestrator.GetApi(apiKey);

            var routeInfo = apiInfo.Mediator.GetRoute(key);

            if (routeInfo.Exec != null)
            {
                return Ok(await routeInfo.Exec(apiInfo, routeInfo, this.Request));
            }
            else
            {
                using (var client = routeInfo.HttpClientConfig?.HttpClient() ?? new HttpClient())
                {
                    this.Request.Headers?.AddRequestHeaders(client.DefaultRequestHeaders);

                    var response = await client.GetAsync($"{apiInfo.BaseUrl}{routeInfo.Path}{parameters}");

                    response.EnsureSuccessStatusCode();

                    return Ok(JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), routeInfo.ResponseType));
                }
            }
        }
    }
}