using BuildingBlocks.Web.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.Web.Extensions;

public static class MiddlewaresExtensions
{
    public static IApplicationBuilder UsePropagateCorrelationIdMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PropagateCorrelationIdMiddleware>();
    }

    public static IApplicationBuilder UseEndpointLoggingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<EndpointLoggingMiddleware>();
    }
    public static IApplicationBuilder UseDeChunkerMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<DeChunkerMiddleware>();
    }
}
