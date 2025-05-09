using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notification.Infrastructure.Context;

namespace Notification.Infrastructure.Shared.Extensions.WebApplicationExtensions
{
    public static partial class WebApplicationExtensions
    {
        public static async Task ApplyDatabaseMigrationsAsync(this WebApplication app)
        {
            var configuration = app.Services.GetRequiredService<IConfiguration>();

            if (configuration.GetValue<bool>("PostgresOptions:UseInMemory") == false)
            {
                using var serviceScope = app.Services.CreateScope();
                var locationDbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                app.Logger.LogInformation("Updating notification database...");

                await locationDbContext.Database.MigrateAsync();

                app.Logger.LogInformation("Updated notification database");
            }
        }
    }
}
