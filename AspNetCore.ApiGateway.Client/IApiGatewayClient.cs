using Microsoft.AspNetCore.JsonPatch;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Client
{
    public interface IApiGatewayClient
    {
        Task<TResponse> GetAsync<TResponse>(ApiGatewayParameters parameters);
        Task<TResponse> PostAsync<TPayload, TResponse>(ApiGatewayParameters parameters, TPayload data);
        Task<TResponse> PutAsync<TPayload, TResponse>(ApiGatewayParameters parameters, TPayload data);
        Task<TResponse> PatchAsync<TPayload, TResponse>(ApiGatewayParameters parameters, JsonPatchDocument<TPayload> data)
            where TPayload : class;
        Task<TResponse> DeleteAsync<TResponse>(ApiGatewayParameters parameters);
    }
}