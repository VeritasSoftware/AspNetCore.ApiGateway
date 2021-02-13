using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway.Application.ExceptionFilters
{
    public interface IGatewayExceptionFilter
    {
        Task OnExceptionAsync(ExceptionContext context, string api, string key, string verb);
    }

    public interface IGatewayVerbExceptionFilter
    {
        Task OnExceptionAsync(ExceptionContext context, string api, string key);
    }

    public interface IGetOrHeadGatewayExceptionFilter : IGatewayVerbExceptionFilter
    {
    }

    public interface IGetOrchestrationGatewayExceptionFilter : IGatewayVerbExceptionFilter
    {
    }

    public interface IPostGatewayExceptionFilter : IGatewayVerbExceptionFilter
    {
    }

    public interface IHubPostGatewayExceptionFilter : IGatewayVerbExceptionFilter
    {
    }

    public interface IPutGatewayExceptionFilter : IGatewayVerbExceptionFilter
    {
    }

    public interface IPatchGatewayExceptionFilter : IGatewayVerbExceptionFilter
    {
    }

    public interface IDeleteGatewayExceptionFilter : IGatewayVerbExceptionFilter
    {
    }
}
