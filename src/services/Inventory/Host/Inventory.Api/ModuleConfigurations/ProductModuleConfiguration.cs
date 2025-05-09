using System.Threading.Tasks;
using Asp.Versioning;
using Asp.Versioning.Builder;
using BuildingBlocks.Abstractions.Web.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Inventory.Api.ModuleConfigurations
{
    internal class ProductModuleConfiguration : IModuleConfiguration
    {
        public const string Tag = "products";
        public const string PrefixUri = "";
        public const string TextPagingDefaults = "Default Page=1, Default PageSize=10, Default Sorts=null, Default Filters=null, Default Includes=null";
        public static readonly string RateLimitPolicyName = "SecurityPolicy.RateLimiting.FixedWindow";
        public static ApiVersionSet VersionSet { get; private set; } = default!;

        public WebApplicationBuilder AddModuleServices(WebApplicationBuilder builder)
        {
            return builder;
        }

        public Task<WebApplication> ConfigureModule(WebApplication app)
        {
            return Task.FromResult(app);
        }

        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            VersionSet = endpoints.NewApiVersionSet(Tag)
                .HasApiVersion(new ApiVersion(1, 0))
                .HasApiVersion(new ApiVersion(2, 0))
                .ReportApiVersions()
                .Build();

            return endpoints;
        }
    }
}
