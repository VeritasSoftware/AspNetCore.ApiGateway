using System.Collections.Generic;

namespace AspNetCore.ApiGateway.Client
{
    public class ApiGatewayParameters
    {
        public string Api { get; set; }
        public string Key { get; set; }
        public string Parameters { get; set; }
        public Dictionary<string, IEnumerable<string>> HeaderLists { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}
