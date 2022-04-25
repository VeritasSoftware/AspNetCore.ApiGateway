using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Client
{
    public interface IApiGatewayClient
    {
        Task<TResponse> GetAsync<TResponse>(string api, string key, string parameters = null);
    }
}