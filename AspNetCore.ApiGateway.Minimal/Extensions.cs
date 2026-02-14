using AspNetCore.ApiGateway.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AspNetCore.ApiGateway.Minimal
{
    public static class Extensions
    {
        static ApiGatewayOptions Options { get; set; }

        private static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
        {
            var builder = new ServiceCollection()
                .AddLogging()
                .AddMvc()
                .AddNewtonsoftJson()
                .Services.BuildServiceProvider();

            return builder
                .GetRequiredService<IOptions<MvcOptions>>()
                .Value
                .InputFormatters
                .OfType<NewtonsoftJsonPatchInputFormatter>()
                .First();
        }

        public static void AddApiGateway(this IServiceCollection services, Action<ApiGatewayOptions> options = null)
        {
            Options = new ApiGatewayOptions();

            options?.Invoke(Options);
            
            services.AddSingleton<IMediator, Mediator>();

            services.AddSingleton<IApiOrchestrator>(sp => new ApiOrchestrator(
                sp.GetRequiredService<IMediator>()));

            services.AddScoped<IApiGatewayRequestProcessor, ApiGatewayRequestProcessor>();

            if (Options.DefaultMyHttpClientHandler != null)
            {
                services
                    .AddHttpClient<IHttpService, HttpService>()
                    .ConfigurePrimaryHttpMessageHandler(Options.DefaultMyHttpClientHandler);
            }
            else if (Options.DefaultHttpClientConfigure != null)
            {
                services.AddHttpClient<IHttpService, HttpService>(Options.DefaultHttpClientConfigure);
            }
            else
            {
                services.AddHttpClient<IHttpService, HttpService>();
            }            

            services.AddApiGatewayResponseCaching();

            services.AddControllers()
                    .AddNewtonsoftJson(); // Enables support for JsonPatchDocument

            services.AddControllers(options =>
            {
                options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
            });
        }

        public static void UseApiGateway(this IApplicationBuilder app, Action<IApiOrchestrator> setApis)
        {
            var serviceProvider = app.ApplicationServices;
            var apiOrchestrator = serviceProvider.GetService<IApiOrchestrator>();
            setApis(apiOrchestrator);
            if (Options != null)
            {
                if (Options.UseResponseCaching)
                {
                    app.UseResponseCaching();
                }
            }
            app.UseMiddleware<GatewayMiddleware>();

            var gatewayMiddleware = serviceProvider.GetServiceOrNull<IGatewayMiddleware>();

            if (gatewayMiddleware != null)
            {
                app.UseMiddleware<GatewayMiddlewareService>();
            }          
        }

        internal static T GetServiceOrNull<T>(this IServiceProvider serviceProvider)
            where T: class
        {
            try
            {
                return serviceProvider.GetService<T>();
            }
            catch(Exception)
            {
                return null;
            }
        }

        internal static void AddApiGatewayResponseCaching(this IServiceCollection services)
        {
            if (Options != null)
            {
                if (Options.UseResponseCaching && Options.ResponseCacheSettings != null)
                {
                    services.AddResponseCaching();

                    services.AddScoped(sp => new ResponseCacheTillAttribute(sp.GetRequiredService<IHttpContextAccessor>(),
                                                                            sp.GetRequiredService<IApiOrchestrator>(),
                                                                            Options)
                    {
                        NoStore = Options.ResponseCacheSettings.NoStore,
                        Location = Options.ResponseCacheSettings.Location,
                        VaryByHeader = Options.ResponseCacheSettings.VaryByHeader,
                        VaryByQueryKeys = Options.ResponseCacheSettings.VaryByQueryKeys,
                        CacheProfileName = Options.ResponseCacheSettings.CacheProfileName
                    });
                }
                else
                {
                    services.AddScoped(sp => new ResponseCacheTillAttribute(sp.GetRequiredService<IHttpContextAccessor>(),
                                                                            sp.GetRequiredService<IApiOrchestrator>(),
                                                                            Options)
                    {
                        NoStore = true
                    });
                }
            }
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

        internal static ApiOrchestration FilterRoutes(this ApiOrchestration orchestration, string key)
        {
            orchestration.ApiRoutes = orchestration.ApiRoutes.Where(y => y.RouteKey.Contains(key.Trim())).ToList();
            orchestration.Routes = orchestration.ApiRoutes;
            return orchestration;
        }

        internal static string ToUtcLongDateTime(this DateTime dateTime)
        {
            return dateTime.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
        }

        internal static void LogApiInfo(this ILogger<ApiGatewayLog> logger, string api, string key, string parameters, object request = null)
        {
            if (request != null)
                logger.LogInformation($"ApiGateway: Incoming POST request. api: {api}, key: {key}, object: {JsonSerializer.Serialize(request)}, parameters: {parameters}, UtcTime: { DateTime.UtcNow.ToUtcLongDateTime() }");
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
