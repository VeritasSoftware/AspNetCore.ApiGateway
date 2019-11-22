using AspNetCore.ApiGateway;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiGateway.API
{
    public class WeatherService : IWeatherService
    {
        public HttpClientConfig GetClient()
        {
            return new HttpClientConfig()
            {
                HttpClient = () => new HttpClient()
            };
        }

        public async Task<object> GetTypes(ApiInfo apiInfo, RouteInfo routeInfo, HttpRequest request)
        {
            return await Task.FromResult(new[]
                                    {
                                        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
                                    });
        }
    }
}
