using System;
using System.Net.Http;

namespace AspNetCore.ApiGateway
{
    public class ApiGatewayOptions
    {
        public bool UseResponseCaching { get; set; }
        public ApiGatewayResponseCacheSettings ResponseCacheSettings { get; set; }
        public Action<IServiceProvider, HttpClient> DefaultHttpClientConfigure { get; set; }
        public Func<IServiceProvider, MyHttpClient> DefaultMyHttpClient { get; set; }
    }

    public class MyHttpClient: IHttpService
    {
        public HttpClient Client { get; private set; }

        public MyHttpClient(HttpClientHandler clientHandler)
        {
            Client = new HttpClient(clientHandler);
        }
    }
}
