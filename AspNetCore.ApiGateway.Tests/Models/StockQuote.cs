using System.Text.Json.Serialization;

namespace AspNetCore.ApiGateway.Tests
{
    public class StockQuote
    {
        [JsonPropertyName("companyName")]
        public string CompanyName { get; set; }

        [JsonPropertyName("costPerShare")]
        public string CostPerShare { get; set; }
    }
}
