namespace AspNetCore.ApiGateway.Application
{
    public class GatewayHubUser
    {
        public string Api { get; set; }
        public string Key { get; set; }
        public string UserId { get; set; }
        public string ReceiveKey { get; set; }
    }

    internal class GatewayHubUserExtended : GatewayHubUser
    {
        public string ConnectionId { get; set; }
    }
}
