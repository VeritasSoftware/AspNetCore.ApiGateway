using System.Net.Http;

namespace AspNetCore.ApiGateway
{
    public interface IHttpService
    {
        HttpClient Client { get; }
    }

    public class HttpService : IHttpService
    {
        public HttpService(HttpClient client)
        {
            this.Client = client;
        }

        public HttpClient Client { get; }
    }

    public class MyHttpClient : IHttpService
    {
        public HttpClient Client { get; private set; }

        public MyHttpClient(HttpClientHandler clientHandler)
        {
            Client = new HttpClient(clientHandler);
        }
    }
}
