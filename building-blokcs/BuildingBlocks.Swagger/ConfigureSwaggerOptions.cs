using System.Text;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BuildingBlocks.Swagger
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider provider;
        private readonly SwaggerOptions? _options;
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IOptions<SwaggerOptions> options)
        {
            this.provider = provider;
            _options = options.Value;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            string name = _options?.Name ?? "Name";
            var text = new StringBuilder(name);
            var info = new OpenApiInfo
            {
                Version = description.ApiVersion.ToString(),
                Title = _options?.Title ?? "Title",
                Description = _options?.Description?? "Description",
                Contact = new OpenApiContact { Name = _options?.ContactName?? "", Email = _options?.ContactEmail ?? "" },
                License = new OpenApiLicense { Name = _options?.LicenseName?? "", Url = new Uri(_options?.LicenseUrl??  "") }
            };

            if (description.IsDeprecated)
            {
                text.Append("This API version has been deprecated.");
            }

            if (description.SunsetPolicy is SunsetPolicy policy)
            {
                if (policy.Date is DateTimeOffset when)
                {
                    text.Append(" The API will be sunset on ").Append(when.Date.ToShortDateString()).Append('.');
                }

                if (policy.HasLinks)
                {
                    text.AppendLine();

                    for (var i = 0; i < policy.Links.Count; i++)
                    {
                        var link = policy.Links[i];

                        if (link.Type == "text/html")
                        {
                            text.AppendLine();

                            if (link.Title.HasValue)
                            {
                                text.Append(link.Title.Value).Append(": ");
                            }

                            text.Append(link.LinkTarget.OriginalString);
                        }
                    }
                }
            }

            info.Description = text.ToString();

            return info;
        }
    }
}
