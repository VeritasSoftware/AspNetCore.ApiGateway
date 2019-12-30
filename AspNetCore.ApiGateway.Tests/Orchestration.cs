using System.Collections.Generic;

namespace AspNetCore.ApiGateway.Tests
{
    public class Orchestration
    {
        public string Api { get; set; }

        public IEnumerable<Route> Routes { get; set; }
    }

    public class Route
    {
        public string Key { get; set; }

        public string Verb { get; set; }

        public string Request { get; set; }

        public string Response { get; set; }
    }
}
