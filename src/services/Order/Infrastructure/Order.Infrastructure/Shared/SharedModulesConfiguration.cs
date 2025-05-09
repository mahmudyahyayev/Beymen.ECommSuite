using BuildingBlocks.Abstractions.Web.Module;
using BuildingBlocks.Core;
using Inventory.Infrastructure.Shared.Extensions.WebApplicationExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Order.Infrastructure.Shared.Extensions.WebApplicationBuilderExtensions;
using Order.Infrastructure.Shared.Extensions.WebApplicationExtensions;

namespace Order.Infrastructure.Shared
{
    public class SharedModulesConfiguration : ISharedModulesConfiguration
    {
        public WebApplicationBuilder AddSharedModuleServices(WebApplicationBuilder builder)
        {
            builder.AddInfrastructure();
            builder.AddOptions();
            builder.AddStorage();
            return builder;
        }

        public async Task<WebApplication> ConfigureSharedModule(WebApplication app)
        {
            await app.UseInfrastructureAsync();
            ServiceActivator.Configure(app.Services);
            await app.ApplyDatabaseMigrationsAsync();
            await app.SeedDataAsync();
            return app;
        }

        public IEndpointRouteBuilder MapSharedModuleEndpoints(IEndpointRouteBuilder endpoints)
        {
            return endpoints;
        }
    }
}
