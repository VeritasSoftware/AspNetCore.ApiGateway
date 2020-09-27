using AspNetCore.ApiGateway.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http.Headers;

namespace AspNetCore.ApiGateway
{
    public static class Extensions
    {
        static ApiGatewayOptions Options { get; set; }

        public static void AddApiGateway(this IServiceCollection services, Action<ApiGatewayOptions> options = null)
        {
            Options = new ApiGatewayOptions();

            options?.Invoke(Options);

            var apis = new ApiOrchestrator();
            
            services.AddTransient<IApiOrchestrator>(x => apis);
            services.AddScoped<GatewayAuthorizeAttribute>();
            services.AddScoped<GatewayGetOrHeadAuthorizeAttribute>();
            services.AddScoped<GatewayGetOrchestrationAuthorizeAttribute>();
            services.AddScoped<GatewayPostAuthorizeAttribute>();
            services.AddScoped<GatewayHubPostAuthorizeAttribute>();
            services.AddScoped<GatewayPutAuthorizeAttribute>();
            services.AddScoped<GatewayPatchAuthorizeAttribute>();
            services.AddScoped<GatewayDeleteAuthorizeAttribute>();
            services.AddHttpClient<IHttpService, HttpService>();

            if (Options != null)
            {
                if (Options.UseResponseCaching && Options.ResponseCacheSettings != null)
                {
                    services.AddResponseCaching();

                    services.AddMvc(o => o.Filters.Add(new ResponseCacheAttribute 
                    { 
                        NoStore = Options.ResponseCacheSettings.NoStore, 
                        Location = Options.ResponseCacheSettings.Location,
                        Duration = Options.ResponseCacheSettings.Duration,
                        VaryByHeader = Options.ResponseCacheSettings.VaryByHeader,
                        VaryByQueryKeys = Options.ResponseCacheSettings.VaryByQueryKeys,
                        CacheProfileName = Options.ResponseCacheSettings.CacheProfileName
                    }));
                }
            }

            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);            
        }

        public static void UseApiGateway(this IApplicationBuilder app, Action<IApiOrchestrator> setApis)
        {
            var serviceProvider = app.ApplicationServices;
            setApis(serviceProvider.GetService<IApiOrchestrator>());
            if (Options != null)
            {
                if (Options.UseResponseCaching)
                {
                    app.UseResponseCaching();
                }
            }
            app.UseMiddleware<GatewayMiddleware>();
        }

        internal static void AddRequestHeaders (this IHeaderDictionary requestHeaders, HttpRequestHeaders headers)
        {
            foreach (var item in requestHeaders)
            {
                try
                {
                    if (!headers.Contains(item.Key))
                        headers.Add(item.Key, item.Value.ToString());
                }
                catch(Exception)
                { }
            }
        }

        internal static Orchestration FilterRoutes(this Orchestration orchestration, string key)
        {
            orchestration.Routes = orchestration.Routes.Where(y => y.Key.Contains(key.Trim()));            
            return orchestration;
        }

        internal static string ToUtcLongDateTime(this DateTime dateTime)
        {
            return dateTime.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
        }

        internal static void LogApiInfo(this ILogger<ApiGatewayLog> logger, string api, string key, string parameters, object request = null)
        {
            if (request != null)
                logger.LogInformation($"ApiGateway: Incoming POST request. api: {api}, key: {key}, object: {JsonConvert.SerializeObject(request)}, parameters: {parameters}, UtcTime: { DateTime.UtcNow.ToUtcLongDateTime() }");
            else
                logger.LogInformation($"ApiGateway: Incoming POST request. api: {api}, key: {key}, UtcTime: { DateTime.UtcNow.ToUtcLongDateTime() }");
        }

        internal static void LogApiInfo(this ILogger<ApiGatewayLog> logger, string url, bool beforeBackendCall = true)
        {
            if (beforeBackendCall)
                logger.LogInformation($"ApiGateway: Calling back end. Url: {url}, UtcTime: { DateTime.UtcNow.ToUtcLongDateTime() }");
            else
                logger.LogInformation($"ApiGateway: Finished calling back end. Url: {url}, UtcTime: { DateTime.UtcNow.ToUtcLongDateTime() }");
        }

    }
}
