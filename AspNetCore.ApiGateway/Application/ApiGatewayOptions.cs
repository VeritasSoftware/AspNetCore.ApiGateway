namespace AspNetCore.ApiGateway
{
    public class ApiGatewayOptions
    {
        public bool UseResponseCaching { get; set; }
        public ApiGatewayResponseCacheSettings ResponseCacheSettings { get; set; }
    }
}
