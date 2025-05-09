using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;
using System.Security.Claims;

namespace BuildingBlocks.Logging
{
    public static class LogEnricher
    {
        public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var request = httpContext.Request;
            diagnosticContext.Set("Host", request.Host);
            diagnosticContext.Set("Protocol", request.Protocol);
            diagnosticContext.Set("Scheme", request.Scheme);
            if (request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", request.QueryString.Value);
            }

            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);
            diagnosticContext.Set("UserId", httpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            var endpoint = httpContext.GetEndpoint();
            if (endpoint is not null)
            {
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            }
        }
        public static LogEventLevel GetLogLevel(HttpContext ctx, double _, Exception? ex) =>
            ex != null
                ? LogEventLevel.Error
                : ctx.Response.StatusCode > 499
                    ? LogEventLevel.Error
                    : IsHealthCheckEndpoint(ctx) || IsSwagger(ctx)
                        ? LogEventLevel.Debug
                        : LogEventLevel.Information;

        private static bool IsSwagger(HttpContext ctx)
        {
            var isHealth = ctx.Request.Path.Value?.Contains("swagger", StringComparison.Ordinal) ?? false;

            return isHealth;
        }

        private static bool IsHealthCheckEndpoint(HttpContext ctx)
        {
            var isHealth = ctx.Request.Path.Value?.Contains("health", StringComparison.Ordinal) ?? false;

            return isHealth;
        }
    }
}
