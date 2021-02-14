using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Application.ResultFilters
{
    public interface IGatewayResultFilter
    {
        Task OnResultExecutionAsync(ResultExecutingContext context, string api, string key, string verb);
    }

    public interface IGatewayVerbResultFilter
    {
        Task OnResultExecutionAsync(ResultExecutingContext context, string api, string key);
    }

    public interface IGetOrHeadGatewayResultFilter : IGatewayVerbResultFilter
    {
    }

    public interface IGetOrchestrationGatewayResultFilter : IGatewayVerbResultFilter
    {
    }

    public interface IPostGatewayResultFilter : IGatewayVerbResultFilter
    {
    }

    public interface IHubPostGatewayResultFilter : IGatewayVerbResultFilter
    {
    }

    public interface IPutGatewayResultFilter : IGatewayVerbResultFilter
    {
    }

    public interface IPatchGatewayResultFilter : IGatewayVerbResultFilter
    {
    }

    public interface IDeleteGatewayResultFilter : IGatewayVerbResultFilter
    {
    }
}
