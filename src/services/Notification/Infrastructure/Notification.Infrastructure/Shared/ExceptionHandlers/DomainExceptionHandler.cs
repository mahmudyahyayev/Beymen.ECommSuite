using BuildingBlocks.Abstractions.Correlation;
using BuildingBlocks.Core.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Notification.Infrastructure.Shared.ExceptionHandlers;

public sealed class DomainExceptionHandler : IExceptionHandler
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public DomainExceptionHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
            return false;

        var env = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();
        var logger = httpContext.RequestServices.GetRequiredService<ILogger<DomainExceptionHandler>>();
        var correlationService = httpContext.RequestServices.GetRequiredService<ICorrelationService>();

        var problemDetails = new ExtensionProblemDetails()
        {
            CorrelationId = correlationService.CorrelationId,
            StatusCode = domainException.ErrorModel.StatusCode,
            ErrorList = [new ErrorList()
                    {
                        Message = domainException.Message,
                        Code = domainException.ErrorModel.ExceptionId
                    }]
        };

        httpContext.Response.StatusCode = problemDetails.StatusCode;

        if (env.IsDevelopment() || env.IsStaging())
        {
            problemDetails.Detail = exception.Message;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        logger.LogError(
            "ExceptionHandlerMiddleware: handled exception {ExceptionType} with status code {StatusCode} and message {Message}. ProblemDetails: {ResponseBody}",
            problemDetails.Title,
            problemDetails.Status,
            problemDetails.Detail,
            JsonConvert.SerializeObject(exception)
        );
        
        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
