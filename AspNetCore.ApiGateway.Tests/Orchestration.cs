using System.Collections.Generic;

namespace AspNetCore.ApiGateway.Tests
{
    public class Orchestration
    {
        public string Api { get; set; }

        public IList<string> RouteKeys { get; set; }
    }
}
