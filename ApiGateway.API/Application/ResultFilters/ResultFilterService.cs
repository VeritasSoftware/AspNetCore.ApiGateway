using AspNetCore.ApiGateway.Application.ResultFilters;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace ApiGateway.API.Application.ResultFilters
{
    public class ResultFilterService : IGatewayResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, string apiKey, string routeKey, string verb)
        {
            //modify result here

            await Task.CompletedTask;
        }
    }

    public class PostResultFilterService : IPostGatewayResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, string apiKey, string routeKey)
        {
            //modify result here

            await Task.CompletedTask;
        }
    }
}
