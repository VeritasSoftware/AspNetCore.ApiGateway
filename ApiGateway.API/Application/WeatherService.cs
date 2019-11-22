using AspNetCore.ApiGateway;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiGateway.API
{
    public class WeatherService : IWeatherService
    {
        /// <summary>
        /// If you want to use a custom HttpClient or HttpContent for your backend Api call, you can do this.
        /// </summary>
        /// <returns><see cref="HttpClientConfig"/></returns>
        public HttpClientConfig GetClient()
        {
            return new HttpClientConfig()
            {
                HttpClient = () => new HttpClient()
            };
        }

        /// <summary>
        /// If you want to completely customize your backend Api call, you can do this
        /// </summary>
        /// <param name="apiInfo">The api info</param>
        /// <param name="routeInfo">The route info</param>
        /// <param name="request">The gateway's incoming request</param>
        /// <returns></returns>
        public async Task<object> GetTypes(ApiInfo apiInfo, RouteInfo routeInfo, HttpRequest request)
        {
            return await Task.FromResult(new[]
                                    {
                                        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
                                    });
        }
    }
}
