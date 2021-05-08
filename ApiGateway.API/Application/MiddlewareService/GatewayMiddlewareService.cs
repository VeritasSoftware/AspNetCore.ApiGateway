using AspNetCore.ApiGateway.Middleware;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGateway.API.Application.MiddlewareService
{
    public class GatewayMiddlewareService : IGatewayMiddleware
    {
        public async Task Invoke(HttpContext context, string api, string key)
        {
            //do your work here

            await Task.CompletedTask;
        }
    }
}
