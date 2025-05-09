using Asp.Versioning;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BuildingBlocks.Swagger
{
    public class ApiVersionOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var actionMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
            operation.Parameters ??= new List<OpenApiParameter>();

            var apiVersionMetadata = actionMetadata.Any(metadataItem => metadataItem is ApiVersionMetadata);
        }
    }
}
