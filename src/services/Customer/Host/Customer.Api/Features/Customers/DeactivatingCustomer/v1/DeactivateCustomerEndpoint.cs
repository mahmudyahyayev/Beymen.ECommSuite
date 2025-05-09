using AutoMapper;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using Customer.Api.ModuleConfigurations;
using Customer.Application.Features.Customers.DeactivatingCustomer.v1;
using Swashbuckle.AspNetCore.Annotations;

namespace Customer.Api.Features.Customers.DeactivatingCustomer.v1;

internal class DeactivateCustomerEndpoint : ICommandMinimalEndpoint<Guid, IResult>
{
    public string GroupName => CustomerModuleConfiguration.Tag;
    public string PrefixRoute => CustomerModuleConfiguration.PrefixUri;
    public double Version => 1.0;
    public string TextPagingDefaults => CustomerModuleConfiguration.TextPagingDefaults;
    public string RateLimitPolicyName => CustomerModuleConfiguration.RateLimitPolicyName;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        return builder
            .MapPatch("/api/customers/{id}", HandleAsync)
            //.RequireAuthorization()
            .WithMetadata(new SwaggerOperationAttribute("Deactivating Customer", "Deactivating Customer"))
            .WithName($"DeactivateCustomer{Version}")
            .WithDisplayName("Deactivating Customer")
            .RequireRateLimiting(RateLimitPolicyName)
            .WithApiVersionSet(CustomerModuleConfiguration.VersionSet)
            .MapToApiVersion(Version);
    }

    public async Task<IResult> HandleAsync(
        HttpContext context,
        Guid id,
        ICommandProcessor commandProcessor,
        IMapper mapper,
        CancellationToken cancellationToken)
    {

        var command = new DeactivateCustomer(id);

        using (Serilog.Context.LogContext.PushProperty("Endpoint", nameof(DeactivateCustomerEndpoint)))
        {
            await commandProcessor.SendAsync(command, cancellationToken);

            return Results.Ok();
        }
    }
}

