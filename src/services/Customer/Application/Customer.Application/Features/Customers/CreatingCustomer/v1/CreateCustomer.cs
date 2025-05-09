using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Customer.Application.Features.Customers.CreatingCustomer.v1;

public record CreateCustomer(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email) : ITxCreateCommand<bool>;