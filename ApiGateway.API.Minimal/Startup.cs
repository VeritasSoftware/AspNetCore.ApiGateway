using AspNetCore.ApiGateway.Application;
using AspNetCore.ApiGateway.Minimal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;

namespace ApiGateway.API.Minimal
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
            //Hook up GatewayHub using SignalR
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin();
            }));

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

            services.AddEndpointsApiExplorer(); // Required for Minimal APIs
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My Minimal API Gateway",
                    Description = "A simple example of ASP.NET Core Minimal API with Swagger",
                    Version = "v1"
                });
            });

            services.AddMvc();            
        }

        //public void Configure(IApplicationBuilder app, WebApplication webApplication)
        public void Configure(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Minimal API Gateway V1");
                // Optional: set the UI to load at the app root URL
                // c.RoutePrefix = string.Empty; 
            });

            //webApplication.
            app.UseCors("CorsPolicy");

            //Api gateway
            app.UseApiGateway(orchestrator => ApiOrchestration.Create(orchestrator, app));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Api Gateway Minimal API
                endpoints.MapApiGatewayHead();
                endpoints.MapApiGatewayGet();
                endpoints.MapApiGatewayPost();
                endpoints.MapApiGatewayPut();
                endpoints.MapApiGatewayDelete();
                endpoints.MapApiGatewayPatch();
            });
        }
    }
}
