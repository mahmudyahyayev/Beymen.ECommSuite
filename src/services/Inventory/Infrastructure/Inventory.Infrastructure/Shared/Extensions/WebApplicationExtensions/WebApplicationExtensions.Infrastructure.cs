using BuildingBlocks.Core.Web.Extensions;
using BuildingBlocks.HealthCheck;
using BuildingBlocks.Logging;
using BuildingBlocks.Messaging.Persistence.Postgres.Extensions;
using BuildingBlocks.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Inventory.Infrastructure.Shared.Extensions.WebApplicationExtensions
{
    public static partial class WebApplicationExtensions
    {
        public static async Task UseInfrastructureAsync(this WebApplication app)
        {
            app.UseSerilogRequestLogging(opts =>
            {
                opts.EnrichDiagnosticContext = LogEnricher.EnrichFromRequest;
                opts.GetLevel = LogEnricher.GetLogLevel;
            });

            app.UsePropagateCorrelationIdMiddleware();
            app.UseEndpointLoggingMiddleware();
            app.UseDeChunkerMiddleware();
            app.UseExceptionHandler(options => { });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();

            if (!app.Environment.IsTest())
            {
                app.UseCustomHealthCheck();
            }

            await app.UsePostgresPersistenceMessageAsync(app.Logger);
        }
    }
}
