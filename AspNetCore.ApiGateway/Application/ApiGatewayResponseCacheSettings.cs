using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.ApiGateway
{
    public class ApiGatewayResponseCacheSettings
    {
        public int Duration { get; set; }
        public ResponseCacheLocation Location { get; set; }
        public bool NoStore { get; set; }
        public string VaryByHeader { get; set; }

        public string[] VaryByQueryKeys { get; set; }
    }
}
