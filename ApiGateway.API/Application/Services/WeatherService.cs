using AspNetCore.ApiGateway;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiGateway.API.Application.Services
{
    public class WeatherService : IWeatherService
    {
        /// <summary>
        /// If you want to customize the default HttpClient or
        /// use your own custom HttpClient or HttpContent 
        /// to hit the backend Api call, you can do this.
        /// </summary>
        /// <returns><see cref="HttpClientConfig"/></returns>
        public HttpClientConfig GetClientConfig()
        {
            return new HttpClientConfig()
            {
                //customize the default HttpClient. eg. add a header.
                CustomizeDefaultHttpClient = (httpClient, request) => httpClient.DefaultRequestHeaders.Add("My header", "My header value"),
                //OR
                //your own custom HttpClient
                HttpClient = () => new HttpClient()
            };
        }

        /// <summary>
        /// If you want to completely customize your backend Api call, you can do this
        /// </summary>
        /// <param name="apiInfo">The api info</param>
        /// <param name="request">The gateway's incoming request</param>
        /// <returns></returns>
        public async Task<object> GetForecast(ApiInfo apiInfo, HttpRequest request)
        {
            //Create your own implementation to hit the backend.
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{apiInfo.BaseUrl}weatherforecast/forecast");

                response.EnsureSuccessStatusCode();

                return JsonSerializer.Deserialize<WeatherForecast[]>(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
