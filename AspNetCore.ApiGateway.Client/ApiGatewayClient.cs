using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Client
{
    public class ApiGatewayClient : IApiGatewayClient
    {
        private readonly HttpClient _httpClient;
        private readonly ApiGatewayClientSettings _settings;

        public ApiGatewayClient(IHttpService httpService, ApiGatewayClientSettings settings)
        {
            _httpClient = httpService.HttpClient;
            _settings = settings;
        }

        public async Task<TResponse> GetAsync<TResponse>(ApiGatewayParameters parameters)
        {
            var gatewayUrl = UrlCombine(_settings.ApiGatewayUrl, parameters.Api, parameters.Key);
            gatewayUrl = $"{gatewayUrl}?parameters={WebUtility.UrlEncode(parameters.Parameters ?? String.Empty)}";

            _httpClient.AddHeaders(parameters);

            var response = await _httpClient.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<TResponse> PostAsync<TPayload, TResponse>(ApiGatewayParameters parameters, TPayload data)
        {
            var gatewayUrl = UrlCombine(_settings.ApiGatewayUrl, parameters.Api, parameters.Key);
            gatewayUrl = $"{gatewayUrl}?parameters={WebUtility.UrlEncode(parameters.Parameters ?? String.Empty)}";

            _httpClient.AddHeaders(parameters);

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(gatewayUrl, content);

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<TResponse> PutAsync<TPayload, TResponse>(ApiGatewayParameters parameters, TPayload data)
        {
            var gatewayUrl = UrlCombine(_settings.ApiGatewayUrl, parameters.Api, parameters.Key);
            gatewayUrl = $"{gatewayUrl}?parameters={WebUtility.UrlEncode(parameters.Parameters ?? String.Empty)}";

            _httpClient.AddHeaders(parameters);

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.PutAsync(gatewayUrl, content);

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<TResponse> PatchAsync<TPayload, TResponse>(ApiGatewayParameters parameters, JsonPatchDocument<TPayload> data)
            where TPayload : class
        {
            var gatewayUrl = UrlCombine(_settings.ApiGatewayUrl, parameters.Api, parameters.Key);
            gatewayUrl = $"{gatewayUrl}?parameters={WebUtility.UrlEncode(parameters.Parameters ?? String.Empty)}";

            _httpClient.AddHeaders(parameters);

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json-patch+json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json-patch+json");

            var method = "PATCH";
            var httpVerb = new HttpMethod(method);

            var httprequest = new HttpRequestMessage
            {
                RequestUri = new Uri(gatewayUrl),
                Content = content,
                Method = httpVerb
            };

            var response = await _httpClient.SendAsync(httprequest);

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<TResponse> DeleteAsync<TResponse>(ApiGatewayParameters parameters)
        {
            var gatewayUrl = UrlCombine(_settings.ApiGatewayUrl, parameters.Api, parameters.Key);
            gatewayUrl = $"{gatewayUrl}?parameters={WebUtility.UrlEncode(parameters.Parameters ?? String.Empty)}";

            _httpClient.AddHeaders(parameters);

            var response = await _httpClient.DeleteAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync());
        }

        private static string UrlCombine(string baseUrl, params string[] segments)
                    => string.Join("/", new[] { baseUrl.TrimEnd('/') }.Concat(segments.Select(s => s.Trim('/'))));
    }
}
