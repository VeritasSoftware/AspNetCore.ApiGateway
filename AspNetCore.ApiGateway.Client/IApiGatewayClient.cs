using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Client
{
    public interface IApiGatewayClient
    {
        Task<TResponse> GetAsync<TResponse>(ApiGatewayParameters parameters);
        Task<TResponse> PostAsync<TPayload, TResponse>(ApiGatewayParameters parameters, TPayload data);
        Task PutAsync<TPayload>(ApiGatewayParameters parameters, TPayload data);
        Task<TResponse> PutAsync<TPayload, TResponse>(ApiGatewayParameters parameters, TPayload data);
        Task PatchAsync<TPayload>(ApiGatewayParameters parameters, JsonPatchDocument<TPayload> data)
            where TPayload : class;
        Task<TResponse> PatchAsync<TPayload, TResponse>(ApiGatewayParameters parameters, JsonPatchDocument<TPayload> data)
            where TPayload : class;
        Task DeleteAsync(ApiGatewayParameters parameters);
        Task<TResponse> DeleteAsync<TResponse>(ApiGatewayParameters parameters);
        Task<IEnumerable<Orchestration>> GetOrchestrationAsync(ApiGatewayParameters parameters);
    }
}