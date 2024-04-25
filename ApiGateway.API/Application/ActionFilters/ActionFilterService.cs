using AspNetCore.ApiGateway.Application.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGateway.API.Application.ActionFilters
{
    public class ActionFilterService : IGatewayActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, string apiKey, string routeKey, string verb)
        {
            //do your work here eg. validation

            //set the result, eg below commented line
            //context.Result = new BadRequestObjectResult(context.ModelState);

            await Task.CompletedTask;
        }
    }

    public class PostActionFilterService : IPostGatewayActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, string apiKey, string routeKey)
        {
            //do your work here eg. validation

            //set the result, eg below commented line
            //context.Result = new BadRequestObjectResult(context.ModelState);

            await Task.CompletedTask;
        }
    }
}
