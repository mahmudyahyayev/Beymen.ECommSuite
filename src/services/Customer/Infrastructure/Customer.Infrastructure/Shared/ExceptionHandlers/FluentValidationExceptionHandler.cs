using BuildingBlocks.Abstractions.Correlation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Customer.Infrastructure.Shared.ExceptionHandlers;

public sealed class FluentValidationExceptionHandler : IExceptionHandler
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public FluentValidationExceptionHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not FluentValidation.ValidationException fluentValidationException)
            return false;

        var env = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();
        var logger = httpContext.RequestServices.GetRequiredService<ILogger<DomainExceptionHandler>>();
        var correlationService = httpContext.RequestServices.GetRequiredService<ICorrelationService>();

        var errorList = new List<ErrorList>();
        if (fluentValidationException?.Errors != null && fluentValidationException.Errors.Any())
        {
            foreach (var error in fluentValidationException.Errors)
            {
                errorList.Add(new ErrorList()
                {
                    Message = error.ErrorMessage,
                    Code = string.Empty
                });
            }
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
