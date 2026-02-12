using AspNetCore.ApiGateway.Minimal;

namespace ApiGateway.API.Minimal
{
    public interface IWeatherService
    {
        HttpClientConfig GetClientConfig();
        Task<object> GetForecast(ApiInfo apiInfo, HttpRequest request);
    }
}