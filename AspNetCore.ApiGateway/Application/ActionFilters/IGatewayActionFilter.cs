using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Application.ActionFilters
{
    public interface IGatewayActionFilter
    {
        Task OnActionExecutionAsync(ActionExecutingContext context, string apiKey, string routeKey, string verb);
    }

    public interface IGatewayVerbActionFilter
    {
        Task OnActionExecutionAsync(ActionExecutingContext context, string apiKey, string routeKey);
    }

    public interface IGetOrHeadGatewayActionFilter : IGatewayVerbActionFilter
    {
    }

    public interface IGetOrchestrationGatewayActionFilter : IGatewayVerbActionFilter
    {
    }

    public interface IPostGatewayActionFilter : IGatewayVerbActionFilter
    {
    }

    public interface IHubPostGatewayActionFilter : IGatewayVerbActionFilter
    {
    }

    public interface IPutGatewayActionFilter : IGatewayVerbActionFilter
    {
    }

    public interface IPatchGatewayActionFilter : IGatewayVerbActionFilter
    {
    }

    public interface IDeleteGatewayActionFilter : IGatewayVerbActionFilter
    {
    }
}
