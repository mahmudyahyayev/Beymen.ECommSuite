using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using Order.Api.ModuleConfigurations;
using Order.Application.Features.Orders.CreatingOrder.v1;
using Swashbuckle.AspNetCore.Annotations;

namespace Order.Api.Features.Orders.CreatingOrder;

internal class CreateOrderEndpoint : ICommandMinimalEndpoint<CreateOrderRequest, IResult>
{
    public string GroupName =>OrderModuleConfiguration.Tag;
    public string PrefixRoute => OrderModuleConfiguration.PrefixUri;
    public double Version => 1.0;
    public string TextPagingDefaults => OrderModuleConfiguration.TextPagingDefaults;
    public string RateLimitPolicyName => OrderModuleConfiguration.RateLimitPolicyName;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        return builder
            .MapPost("/api/orders", HandleAsync)
            //.RequireAuthorization()
            .WithMetadata(new SwaggerOperationAttribute("Creating Order", "Creating Order"))
            .WithName($"CreateOrder{Version}")
            .WithDisplayName("Creating Order")
            .RequireRateLimiting(RateLimitPolicyName)
            .WithApiVersionSet(OrderModuleConfiguration.VersionSet)
            .MapToApiVersion(Version);
    }

    public async Task<IResult> HandleAsync(
        HttpContext context,
        CreateOrderRequest request,
        ICommandProcessor commandProcessor,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var command = new CreateOrder(
            request.CustomerId,
            request.ShippingAddressId,
            request.BillingAddressId,
            request.Items);

        using (Serilog.Context.LogContext.PushProperty("Endpoint", nameof(CreateOrderEndpoint)))
        {
            await commandProcessor.SendAsync(command, cancellationToken);

            return Results.Ok(true);
        }
    }
}

