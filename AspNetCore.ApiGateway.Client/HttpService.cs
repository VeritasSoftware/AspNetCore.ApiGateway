using System.Net.Http;

namespace AspNetCore.ApiGateway.Client
{
    public interface IHttpService
    {
        HttpClient HttpClient { get; }
    }

    public class HttpService : IHttpService
    {
        public HttpClient HttpClient { get; private set; }

        public HttpService(HttpClient httpClient)
        {
            this.HttpClient = httpClient;
        }
    }
}
