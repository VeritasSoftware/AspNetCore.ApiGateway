using AspNetCore.ApiGateway;

namespace ApiGateway.API.Application
{
    public static class Settings
    {
        private static IConfigService _settings = ConfigProvider.MySettings;

        private const string API1 = "API1";
        private const string API1_ROUTE1 = "ROUTE1";

        public static string API1_ApiKey = _settings[API1].ApiKey;
        public static string[] API1_BackendAPIBaseUrls = _settings[API1].BackendAPIBaseUrls;

        public static string API1_ROUTE1_RouteKey = _settings[API1][API1_ROUTE1].RouteKey;
        public static GatewayVerb API1_ROUTE1_Verb = _settings[API1][API1_ROUTE1].Verb;
        public static string API1_ROUTE1_BackendAPIRoutePath = _settings[API1][API1_ROUTE1].BackendAPIRoutePath;
        public static int API1_ROUTE1_ResponseCachingDurationInSeconds = _settings[API1][API1_ROUTE1].ResponseCachingDurationInSeconds;
    }
}
