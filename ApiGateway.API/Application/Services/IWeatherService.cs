using System.Threading.Tasks;
using AspNetCore.ApiGateway;
using Microsoft.AspNetCore.Http;

namespace ApiGateway.API.Application.Services
{
    public interface IWeatherService
    {
        HttpClientConfig GetClientConfig();
        Task<object> GetForecast(ApiInfo apiInfo, HttpRequest request);
    }
}