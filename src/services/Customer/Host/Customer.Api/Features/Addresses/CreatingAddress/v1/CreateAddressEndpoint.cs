using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using Customer.Api.ModuleConfigurations;
using Customer.Application.Features.Addresses.CreatingAddress.v1;
using Swashbuckle.AspNetCore.Annotations;

namespace Customer.Api.Features.Addresses.CreatingAddress.v1;
internal class CreateAddressEndpoint : ICommandMinimalEndpoint<CreateAddressRequest, IResult>
{
    public string GroupName => AddressModuleConfiguration.Tag;
    public string PrefixRoute => AddressModuleConfiguration.PrefixUri;
    public double Version => 1.0;
    public string TextPagingDefaults => AddressModuleConfiguration.TextPagingDefaults;
    public string RateLimitPolicyName => AddressModuleConfiguration.RateLimitPolicyName;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        return builder
            .MapPost("/api/addresses", HandleAsync)
            //.RequireAuthorization()
            .WithMetadata(new SwaggerOperationAttribute("Creating Address", "Creating Address"))
            .WithName($"CreateAddress{Version}")
            .WithDisplayName("Creating Address")
            .RequireRateLimiting(RateLimitPolicyName)
            .WithApiVersionSet(AddressModuleConfiguration.VersionSet)
            .MapToApiVersion(Version);
    }

    public async Task<IResult> HandleAsync(
        HttpContext context,
        CreateAddressRequest request,
        ICommandProcessor commandProcessor,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var command = new CreateAddress(
            request.CustomerId,
            request.TypeId,
            request.Country,
            request.City,
            request.District,
            request.Street,
            request.ZipCode,
            request.BuildingNo,
            request.ApartmentNo,
            request.Floor,
            request.Description);

        using (Serilog.Context.LogContext.PushProperty("Endpoint", nameof(CreateAddressEndpoint)))
        {
            await commandProcessor.SendAsync(command, cancellationToken);

            return Results.Ok(true);
        }
    }
}

