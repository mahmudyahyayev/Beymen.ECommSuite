using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web.Extensions
{
    public static partial class VersioningExtensions
    {
        public static WebApplicationBuilder AddCustomVersioning(this WebApplicationBuilder builder, Action<ApiVersioningOptions>? configurator = null)
        {
            builder.Services.AddMvcCore().AddApiExplorer();

            builder.Services
                .AddApiVersioning(options =>
                {
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.ApiVersionReader = ApiVersionReader.Combine(
                        new HeaderApiVersionReader("api-version"),
                        new QueryStringApiVersionReader(),
                        new UrlSegmentApiVersionReader()
                    );

                    configurator?.Invoke(options);
                })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                })
                .AddMvc();

            return builder;
        }
    }
}
