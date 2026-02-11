using System.Text.Json.Serialization;

namespace ApiGateway.API.Minimal
{
    public class StockQuote
    {
        [JsonPropertyName("companyName")]
        public string CompanyName { get; set; }

        [JsonPropertyName("costPerShare")]
        public string CostPerShare { get; set; }
    }
}
