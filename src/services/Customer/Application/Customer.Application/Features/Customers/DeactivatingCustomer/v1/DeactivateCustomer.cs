using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Customer.Application.Features.Customers.DeactivatingCustomer.v1;
public record DeactivateCustomer(
    Guid Id) : ITxCreateCommand<bool>;

