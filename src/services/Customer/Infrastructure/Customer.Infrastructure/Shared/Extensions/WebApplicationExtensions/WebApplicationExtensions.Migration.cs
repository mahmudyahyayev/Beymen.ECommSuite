using Customer.Infrastructure.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Customer.Infrastructure.Shared.Extensions.WebApplicationExtensions
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

                app.Logger.LogInformation("Updating customer database...");

                await locationDbContext.Database.MigrateAsync();

                app.Logger.LogInformation("Updated customer database");
            }
        }
    }
}
