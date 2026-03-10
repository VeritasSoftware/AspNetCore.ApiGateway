namespace AspNetCore.ApiGateway.AzureFunctions
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
}
