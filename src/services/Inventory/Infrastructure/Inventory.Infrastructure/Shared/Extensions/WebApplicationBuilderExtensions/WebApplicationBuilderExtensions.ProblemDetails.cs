using Inventory.Infrastructure.Shared.ExceptionHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddCustomProblemDetails(this WebApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
        builder.Services.AddExceptionHandler<FluentValidationExceptionHandler>();
        builder.Services.AddExceptionHandler<ArgumentExceptionHandler>();
        builder.Services.AddExceptionHandler<ArgumentNullExceptionHandler>();
        builder.Services.AddExceptionHandler<DomainExceptionHandler>();
        builder.Services.AddExceptionHandler<ApplicationExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        
        return builder;
    }
}
