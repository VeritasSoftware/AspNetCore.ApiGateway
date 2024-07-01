using System;
using System.Net.Http;

namespace AspNetCore.ApiGateway
{
    public class ApiGatewayOptions
    {
        public bool UseResponseCaching { get; set; }
        public ApiGatewayResponseCacheSettings ResponseCacheSettings { get; set; }
        public Action<IServiceProvider, HttpClient> DefaultHttpClientConfigure { get; set; }
        public Func<HttpClientHandler> DefaultMyHttpClientHandler { get; set; }
    }
}
