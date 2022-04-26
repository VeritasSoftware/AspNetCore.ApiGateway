using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;

namespace AspNetCore.ApiGateway.Client
{
    public static class ApiGatewayClientExtensions
    {
        public static IServiceCollection AddApiGatewayClient(this IServiceCollection services, Action<ApiGatewayClientSettings> addSettings)
        {
            services.AddHttpClient<IHttpService, HttpService>();

            var settings = new ApiGatewayClientSettings();
            addSettings(settings);

            services.AddSingleton(settings);

            services.AddScoped<IApiGatewayClient, ApiGatewayClient>();

            return services;
        }

        internal static void AddHeaders(this HttpClient httpClient, ApiGatewayParameters parameters)
        {
            httpClient.DefaultRequestHeaders.Clear();

            if (parameters.Headers != null && parameters.Headers.Any())
            {
                foreach (var header in parameters.Headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            if (parameters.HeaderLists != null && parameters.HeaderLists.Any())
            {
                foreach (var header in parameters.HeaderLists)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
        }
    }
}
