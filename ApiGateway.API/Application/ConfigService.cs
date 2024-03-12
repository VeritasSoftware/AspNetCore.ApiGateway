using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace ApiGateway.API.Application
{
    public class Setting
    {
        public string Identifier { get; set; }
        public string ApiKey { get; set; }
        public string BackendAPIBaseUrl { get; set; }
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
        public string BackendAPIRoutePath { get; set; }
    }

    public interface IConfigService
    {
        Setting this[string identifier] { get; }            
    }

    public class ConfigService : IConfigService
    {
        private IEnumerable<Setting> Settings { get; set; }

        public ConfigService(IConfiguration configuration)
        {
            var settings = configuration.GetSection("Settings")
                                        .Get<List<Setting>>();

            this.Settings = settings;
        }

        public Setting this[string identifier]
        {
            get
            {
                return Settings.Single(s => s.Identifier == identifier);
            }
        }
    }
}
