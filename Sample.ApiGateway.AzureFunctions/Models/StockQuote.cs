using System.Text.Json.Serialization;

namespace Sample.ApiGateway.AzureFunctions
{
    public class StockQuote
    {
        [JsonPropertyName("companyName")]
        public string CompanyName { get; set; }

        [JsonPropertyName("costPerShare")]
        public string CostPerShare { get; set; }
    }
}
