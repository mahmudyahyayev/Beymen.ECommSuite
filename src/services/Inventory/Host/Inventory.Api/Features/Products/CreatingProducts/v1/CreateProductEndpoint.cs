using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using Inventory.Api.ModuleConfigurations;
using Inventory.Application.Features.Products.CreatingProducts.v1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace Inventory.Api.Features.Products.CreatingProducts.v1;

internal class CreateProductEndpoint : ICommandMinimalEndpoint<CreateProductRequest, IResult>
{
    public string GroupName => ProductModuleConfiguration.Tag;
    public string PrefixRoute => ProductModuleConfiguration.PrefixUri;
    public double Version => 1.0;
    public string TextPagingDefaults => ProductModuleConfiguration.TextPagingDefaults;
    public string RateLimitPolicyName => ProductModuleConfiguration.RateLimitPolicyName;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        return builder
            .MapPost("/api/products", HandleAsync)
            //.RequireAuthorization()
            .WithMetadata(new SwaggerOperationAttribute("Creating Product", "Creating Product"))
            .WithName($"CreateProduct{Version}")
            .WithDisplayName("Creating Product")
            .RequireRateLimiting(RateLimitPolicyName)
            .WithApiVersionSet(ProductModuleConfiguration.VersionSet)
            .MapToApiVersion(Version);
    }

    public async Task<IResult> HandleAsync(
        HttpContext context,
        CreateProductRequest request,
        ICommandProcessor commandProcessor,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var command = new CreateProduct(
            request.Name,
            request.Description,
            request.Price,
            request.Stock);

        using (Serilog.Context.LogContext.PushProperty("Endpoint", nameof(CreateProductEndpoint)))
        {
            await commandProcessor.SendAsync(command, cancellationToken);

            return Results.Ok(true);
        }
    }
}

