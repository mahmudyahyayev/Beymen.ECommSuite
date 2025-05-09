using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using Customer.Api.ModuleConfigurations;
using Customer.Application.Features.Customers.CreatingCustomer.v1;
using Swashbuckle.AspNetCore.Annotations;

namespace Customer.Api.Features.Customers.CreatingCustomer.v1
{
    internal class CreateCustomerEndpoint : ICommandMinimalEndpoint<CreateCustomerRequest, IResult>
    {
        public string GroupName => CustomerModuleConfiguration.Tag;
        public string PrefixRoute => CustomerModuleConfiguration.PrefixUri;
        public double Version => 1.0;
        public string TextPagingDefaults => CustomerModuleConfiguration.TextPagingDefaults;
        public string RateLimitPolicyName => CustomerModuleConfiguration.RateLimitPolicyName;

        public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder builder)
        {
            return builder
                .MapPost("/api/customers", HandleAsync)
                //.RequireAuthorization()
                .WithMetadata(new SwaggerOperationAttribute("Creating Customer", "Creating Customer"))
                .WithName($"CreateCustomer{Version}")
                .WithDisplayName("Creating Customer")
                .RequireRateLimiting(RateLimitPolicyName)
                .WithApiVersionSet(CustomerModuleConfiguration.VersionSet)
                .MapToApiVersion(Version);
        }

        public async Task<IResult> HandleAsync(
            HttpContext context,
            CreateCustomerRequest request,
            ICommandProcessor commandProcessor,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new CreateCustomer(
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.Email);

            using (Serilog.Context.LogContext.PushProperty("Endpoint", nameof(CreateCustomerEndpoint)))
            {
                await commandProcessor.SendAsync(command, cancellationToken);

                return Results.Ok(true);
            }
        }
    }
}
