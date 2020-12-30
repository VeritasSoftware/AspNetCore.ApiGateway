using ApiGateway.API.Application.ActionFilters;
using ApiGateway.API.Application.Authorization;
using AspNetCore.ApiGateway;
using AspNetCore.ApiGateway.Application.ActionFilters;
using AspNetCore.ApiGateway.Authorization;
using AspNetCore.ApiGateway.Hubs;
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
            services.AddScoped<IGatewayActionFilter, ValidationActionFilterService>();
            services.AddScoped<IPostGatewayActionFilter, PostValidationActionFilterService>();


            //Api gateway
            services.AddApiGateway(options =>
            {
                options.UseResponseCaching = false;
                options.ResponseCacheSettings = new ApiGatewayResponseCacheSettings
                {
                    Duration = 120,
                    Location = ResponseCacheLocation.Any
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
