using AspNetCore.ApiGateway;

namespace ApiGateway.API.Application
{
    public static class Settings
    {
        private static IConfigService _settings = ConfigProvider.MySettings;

        public static string API1_ApiKey = _settings["API1"].ApiKey;
        public static string[] API1_BackendAPIBaseUrls = _settings["API1"].BackendAPIBaseUrls;

        public static string API1_RouteKey = _settings["API1"]["ROUTE1"].RouteKey;
        public static GatewayVerb API1_Verb = _settings["API1"]["ROUTE1"].Verb;
        public static string API1_BackendAPIRoutePath = _settings["API1"]["ROUTE1"].BackendAPIRoutePath;
    }
}
