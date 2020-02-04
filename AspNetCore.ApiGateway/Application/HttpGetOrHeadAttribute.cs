using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ApiGateway
{
    internal class HttpGetOrHeadAttribute : HttpMethodAttribute
    {
        private static readonly IEnumerable<string> _supportedMethods = new[] { "GET", "HEAD" };

        public HttpGetOrHeadAttribute()
            : base(_supportedMethods)
        { }

        public HttpGetOrHeadAttribute(string template)
            : base(_supportedMethods, template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }
        }
    }
}
