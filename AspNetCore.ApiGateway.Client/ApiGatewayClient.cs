using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        public async Task<TResponse> GetAsync<TResponse>(string api, string key, string parameters = null)
        {
            var gatewayUrl = UrlCombine(_settings.ApiGatewayUrl, api, key);
            gatewayUrl = $"{gatewayUrl}?parameters={WebUtility.UrlEncode(parameters ?? String.Empty)}";

            var response = await _httpClient.GetAsync(gatewayUrl);

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync());
        }

        private static string UrlCombine(string baseUrl, params string[] segments)
                    => string.Join("/", new[] { baseUrl.TrimEnd('/') }.Concat(segments.Select(s => s.Trim('/'))));
    }
}
