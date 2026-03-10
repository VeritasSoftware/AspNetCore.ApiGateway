using AspNetCore.ApiGateway;
using AspNetCore.ApiGateway.AzureFunctions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sample.ApiGateway.AzureFunctions
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IWeatherService, WeatherService>();

            //services.AddSingleton<IApiGatewayConfigService, ApiGatewayConfigService>(); 

            //Api gateway
            services.AddApiGateway(options =>
            {
                options.UseResponseCaching = false;
                options.ResponseCacheSettings = new ApiGatewayResponseCacheSettings
                {
                    Duration = 60, //default for all routes
                    Location = ResponseCacheLocation.Any,
                    //Use VaryByQueryKeys to vary the response for each apiKey & routeKey
                    VaryByQueryKeys = new[] { "apiKey", "routeKey" }
                };
            });                   
        }

        //public void Configure(IApplicationBuilder app, WebApplication webApplication)
        public void Configure(IHost app)
        {
            //Api gateway
            app.UseApiGateway(orchestrator => ApiOrchestration.Create(orchestrator, app));
        }
    }
}
