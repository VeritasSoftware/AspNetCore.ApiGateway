using System.Collections.Generic;

namespace AspNetCore.ApiGateway
{
    public class Orchestration
    {
        public string Api { get; set; }

        public IList<string> RouteKeys { get; set; }
    }
}
