using ApiGateway.API.Application;
using ApiGateway.API.Application.ActionFilters;
using ApiGateway.API.Application.Authorization;
using ApiGateway.API.Application.HubFilters;
using ApiGateway.API.Application.MiddlewareService;
using ApiGateway.API.Application.Services;
using ApiGateway.API.Application.ResultFilters;
using AspNetCore.ApiGateway;
using AspNetCore.ApiGateway.Application.ActionFilters;
using AspNetCore.ApiGateway.Application.ExceptionFilters;
using AspNetCore.ApiGateway.Application.HubFilters;
using AspNetCore.ApiGateway.Application.ResultFilters;
using AspNetCore.ApiGateway.Authorization;
using AspNetCore.ApiGateway.Hubs;
using AspNetCore.ApiGateway.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ApiGateway.API
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
            services.AddSignalR().AddNewtonsoftJsonProtocol();

            services.AddTransient<IWeatherService, WeatherService>();

            //If you want to use the Api Gateway's Authorization, you can do this
            services.AddScoped<IGatewayAuthorization, AuthorizationService>();
            services.AddScoped<IGetOrHeadGatewayAuthorization, GetAuthorizationService>();

            //Action filters
            services.AddScoped<IGatewayActionFilter, ActionFilterService>();
            services.AddScoped<IPostGatewayActionFilter, PostActionFilterService>();

            //Exception filters
            services.AddScoped<IGatewayExceptionFilter, ExceptionFilterService>();
            services.AddScoped<IPostGatewayExceptionFilter, PostExceptionFilterService>();

            //Result filters
            services.AddScoped<IGatewayResultFilter, ResultFilterService>();
            services.AddScoped<IPostGatewayResultFilter, PostResultFilterService>();

            //Hub filters
            services.AddScoped<IGatewayHubFilter, GatewayHubFilterService>();

            //Middleware service
            services.AddTransient<IGatewayMiddleware, GatewayMiddlewareService>();

            services.AddSingleton<IConfigService, ConfigService>(); 

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

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My Api Gateway", Version = "v1" });
            });            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Api Gateway");
            });

            //Api gateway
            app.UseApiGateway(orchestrator => ApiOrchestration.Create(orchestrator, app));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //GatewayHub endpoint
                endpoints.MapHub<GatewayHub>("/gatewayhub");
                endpoints.MapControllers();
            });
        }
    }
}
