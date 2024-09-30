using AspNetCore.ApiGateway;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace ApiGateway.API.Application
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

    public interface IConfigService
    {
        ApiSetting this[string identifier] { get; }            
    }

    public class ConfigService : IConfigService
    {
        private IEnumerable<ApiSetting> Settings { get; set; }

        public ConfigService(IConfiguration configuration)
        {
            var settings = configuration.GetSection("Settings")
                                        .Get<List<ApiSetting>>();

            this.Settings = settings;
        }

        public ApiSetting this[string identifier]
        {
            get
            {
                return Settings.Single(s => s.Identifier == identifier);
            }
        }        
    }

    public static class ConfigProvider
    {
        public static IConfigService MySettings { get; set; }       
    }
}
