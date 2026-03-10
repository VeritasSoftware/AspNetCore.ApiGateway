using AspNetCore.ApiGateway;
using AspNetCore.ApiGateway.AzureFunctions;
using Microsoft.AspNetCore.Http;

namespace Sample.ApiGateway.AzureFunctions
{
    public interface IWeatherService
    {
        HttpClientConfig GetClientConfig();
        Task<object> GetForecast(ApiInfo apiInfo, HttpRequest request);
    }
}