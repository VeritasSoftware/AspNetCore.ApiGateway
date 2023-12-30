namespace AspNetCore.ApiGateway
{
    internal static class GatewayConstants
    {
        public static string GATEWAY_PATH_REGEX = "^/?api/Gateway(/(?!orchestration)(hub/)?(?<apiKey>.*?)/(?<routeKey>.*?)(/.*?)?)?$";
    }
}
