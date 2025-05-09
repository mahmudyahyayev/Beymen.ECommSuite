using BuildingBlocks.Messaging.Persistence.Postgres.MessagePersistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Messaging.Persistence.Postgres.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task UsePostgresPersistenceMessageAsync(this IApplicationBuilder app, ILogger logger)
        {
            await ApplyDatabaseMigrationsAsync(app, logger);
        }

        private static async Task ApplyDatabaseMigrationsAsync(this IApplicationBuilder app, ILogger logger)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var messagePersistenceDbContext =
                serviceScope.ServiceProvider.GetRequiredService<MessagePersistenceDbContext>();

            logger.LogInformation("Applying persistence-message migrations...");

            await messagePersistenceDbContext.Database.MigrateAsync();

            logger.LogInformation("persistence-message migrations applied");
        }
    }
}
