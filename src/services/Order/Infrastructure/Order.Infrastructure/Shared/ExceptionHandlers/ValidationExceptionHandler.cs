using BuildingBlocks.Abstractions.Correlation;
using BuildingBlocks.Core.Validations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Order.Infrastructure.Shared.ExceptionHandlers;

public sealed class ValidationExceptionHandler : IExceptionHandler
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public ValidationExceptionHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
            return false;

        var env = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();
        var logger = httpContext.RequestServices.GetRequiredService<ILogger<DomainExceptionHandler>>();
        var correlationService = httpContext.RequestServices.GetRequiredService<ICorrelationService>();

        var errorList = new List<ErrorList>();
        foreach (var validationError in validationException.ValidationResultModel?.Errors)
        {
            errorList.Add(new ErrorList
            {
                Code = string.Empty,
                Message = validationError.Message,
            });
        }
        var problemDetails = new ExtensionProblemDetails()
        {
            CorrelationId = correlationService.CorrelationId,
            StatusCode = StatusCodes.Status400BadRequest,
            ErrorList = errorList
        };

        httpContext.Response.StatusCode = problemDetails.StatusCode;

        if (env.IsDevelopment() || env.IsStaging())
        {
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
