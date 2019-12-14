using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Stock.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockController : ControllerBase
    {
        private static readonly Dictionary<string, StockQuote> StockQuotes = new Dictionary<string, StockQuote>()
        {
            { "Microsoft", new StockQuote() { CompanyName = "Microsoft", CostPerShare = "300" } },
            { "IBM", new StockQuote() { CompanyName = "IBM", CostPerShare = "200" } }
        };

        private readonly ILogger<StockController> _logger;

        public StockController(ILogger<StockController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<StockQuote> Get()
        {
            return StockQuotes.Select(x => x.Value);
        }

        [HttpGet]
        [Route("{companyName}")]
        public StockQuote Get(string companyName)
        {
            return StockQuotes.SingleOrDefault(x => string.Compare(x.Key, companyName, true) == 0).Value;
        }
    }
}
