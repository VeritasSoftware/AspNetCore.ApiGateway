using AspNetCore.ApiGateway;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.ApiGateway
{
    public class ApiSetting
    {
        public string Identifier { get; set; }
        public string ApiKey { get; set; }
        public string[] BackendAPIBaseUrls { get; set; }
        public RouteSetting[] Routes { get; set; }
        public RouteSetting this[string routeIdentifier]
        {
            get
            {
                return Routes.Single(r => r.Identifier == routeIdentifier);
            }
        }        
    }

    public class RouteSetting
    {
        public string Identifier { get; set; }
        public string RouteKey { get; set; }
        public GatewayVerb Verb { get; set; }
        public string BackendAPIRoutePath { get; set; }
        public int ResponseCachingDurationInSeconds { get; set; } = -1;
    }

    public interface IApiGatewayConfigService
    {
        ApiSetting this[string identifier] { get; }            
    }

    public class ApiGatewayConfigService : IApiGatewayConfigService
    {
        private IEnumerable<ApiSetting> Settings { get; set; }

        public ApiGatewayConfigService(IConfiguration configuration)
        {
            var settings = configuration.GetSection("Settings")
                                        .Get<List<ApiSetting>>();

            this.Settings = settings;

            ApiGatewayConfigProvider.MySettings = this;
        }

        public ApiSetting this[string identifier]
        {
            get
            {
                return Settings.Single(s => s.Identifier == identifier);
            }
        }        
    }

    public static class ApiGatewayConfigProvider
    {
        public static IApiGatewayConfigService MySettings { get; set; }       
    }
}