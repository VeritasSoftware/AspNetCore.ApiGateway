using Microsoft.Extensions.DependencyInjection;
using System;

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
    }
}
