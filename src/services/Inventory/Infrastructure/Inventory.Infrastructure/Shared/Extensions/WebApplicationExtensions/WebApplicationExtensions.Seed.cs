using BuildingBlocks.Abstractions.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Shared.Extensions.WebApplicationExtensions
{
    public static partial class WebApplicationExtensions
    {
        public static async Task SeedDataAsync(this WebApplication app)
        {
            using var serviceScope = app.Services.CreateScope();

            var seeders = serviceScope.ServiceProvider.GetServices<IDataSeeder>();

            foreach (var seeder in seeders.OrderBy(x => x.OrderNumber))
            {
                app.Logger.LogInformation("Seeding '{Seed}' started...", seeder.GetType().Name);

                await seeder.SeedAllAsync(CancellationToken.None);

                app.Logger.LogInformation("Seeding '{Seed}' ended...", seeder.GetType().Name);  
            }
        }
    }
}
