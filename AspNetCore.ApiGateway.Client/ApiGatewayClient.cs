using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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
            var gatewayUrl = UrlCombine(_settings.ApiGatewayBaseUrl, "api/Gateway", parameters.ApiKey, parameters.RouteKey);
            gatewayUrl = $"{gatewayUrl}?parameters={WebUtility.UrlEncode(parameters.Parameters ?? String.Empty)}";

            _httpClient.AddHeaders(parameters);

            var response = await _httpClient.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var returnedContent = await response.Content.ReadAsStringAsync();

            return !string.IsNullOrEmpty(returnedContent) ? JsonSerializer.Deserialize<TResponse>(returnedContent) : default(TResponse);
        }

        public async Task<TResponse> PostAsync<TPayload, TResponse>(ApiGatewayParameters parameters, TPayload data)
        {
            var gatewayUrl = UrlCombine(_settings.ApiGatewayBaseUrl, "api/Gateway", parameters.ApiKey, parameters.RouteKey);
            gatewayUrl = $"{gatewayUrl}?parameters={WebUtility.UrlEncode(parameters.Parameters ?? String.Empty)}";

            _httpClient.AddHeaders(parameters);

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(gatewayUrl, content);

            response.EnsureSuccessStatusCode();

            var returnedContent = await response.Content.ReadAsStringAsync();

            return !string.IsNullOrEmpty(returnedContent) ? JsonSerializer.Deserialize<TResponse>(returnedContent) : default(TResponse);
        }

        public async Task PutAsync<TPayload>(ApiGatewayParameters parameters, TPayload data)
        {
            await this.PutAsync<TPayload, string>(parameters, data);
        }

        public async Task<TResponse> PutAsync<TPayload, TResponse>(ApiGatewayParameters parameters, TPayload data)
        {
            var gatewayUrl = UrlCombine(_settings.ApiGatewayBaseUrl, "api/Gateway", parameters.ApiKey, parameters.RouteKey);
            gatewayUrl = $"{gatewayUrl}?parameters={WebUtility.UrlEncode(parameters.Parameters ?? String.Empty)}";

            _httpClient.AddHeaders(parameters);

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.PutAsync(gatewayUrl, content);

            response.EnsureSuccessStatusCode();

            var returnedContent = await response.Content.ReadAsStringAsync();

            return !string.IsNullOrEmpty(returnedContent) ? JsonSerializer.Deserialize<TResponse>(returnedContent) : default(TResponse);
        }

        public async Task PatchAsync<TPayload>(ApiGatewayParameters parameters, JsonPatchDocument<TPayload> data)
            where TPayload : class
        {
            await this.PatchAsync<TPayload, string>(parameters, data);
        }

        public async Task<TResponse> PatchAsync<TPayload, TResponse>(ApiGatewayParameters parameters, JsonPatchDocument<TPayload> data)
            where TPayload : class
        {
            var gatewayUrl = UrlCombine(_settings.ApiGatewayBaseUrl, "api/Gateway", parameters.ApiKey, parameters.RouteKey);
            gatewayUrl = $"{gatewayUrl}?parameters={WebUtility.UrlEncode(parameters.Parameters ?? String.Empty)}";

            _httpClient.AddHeaders(parameters);

            var content = new StringContent(JsonSerializer.Serialize(data.Operations), Encoding.UTF8, "application/json-patch+json");
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

            var returnedContent = await response.Content.ReadAsStringAsync();

            return !string.IsNullOrEmpty(returnedContent) ? JsonSerializer.Deserialize<TResponse>(returnedContent) : default(TResponse);
        }

        public async Task DeleteAsync(ApiGatewayParameters parameters)
        {
            await this.DeleteAsync<string>(parameters);
        }

        public async Task<TResponse> DeleteAsync<TResponse>(ApiGatewayParameters parameters)
        {
            var gatewayUrl = UrlCombine(_settings.ApiGatewayBaseUrl, "api/Gateway", parameters.ApiKey, parameters.RouteKey);
            gatewayUrl = $"{gatewayUrl}?parameters={WebUtility.UrlEncode(parameters.Parameters ?? String.Empty)}";

            _httpClient.AddHeaders(parameters);

            var response = await _httpClient.DeleteAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var returnedContent = await response.Content.ReadAsStringAsync();

            return !string.IsNullOrEmpty(returnedContent) ? JsonSerializer.Deserialize<TResponse>(returnedContent) : default(TResponse);
        }

        public async Task<IEnumerable<Orchestration>> GetOrchestrationAsync(ApiGatewayParameters parameters)
        {
            var gatewayUrl = UrlCombine(_settings.ApiGatewayBaseUrl, $"api/Gateway/orchestration?api={parameters.ApiKey}&key={parameters.RouteKey}");

            _httpClient.AddHeaders(parameters);

            var response = await _httpClient.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var orchs = JArray.Parse(content);

            var orchestrations = new List<Orchestration>();

            orchs.AsEnumerable().ToList().ForEach(item =>
            {
                var type = item.SelectToken("orchestrationType");
                var orchestrationType = (OrchestationType)Enum.Parse(typeof(OrchestationType), type.ToString());

                switch (orchestrationType)
                {
                    case OrchestationType.Api:
                        orchestrations.Add(item.ToObject<ApiOrchestration>());
                        break;
                    case OrchestationType.Hub:
                        orchestrations.Add(item.ToObject<HubOrchestration>());
                        break;
                    case OrchestationType.EventSource:
                        orchestrations.Add(item.ToObject<EventSourceOrchestration>());
                        break;
                }
            });

            return orchestrations;
        }

        private static string UrlCombine(string baseUrl, params string[] segments)
                    => string.Join("/", new[] { baseUrl.TrimEnd('/') }.Concat(segments.Select(s => s.Trim('/'))));
    }
}
