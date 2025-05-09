using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Notification.Application.Features.Customers.CreatingCustomer.v1.Internals;

public record CreateCustomer(
    Guid CustomerId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email) : ITxCreateCommand<bool>;
