using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Notification.Application.Features.Customers.CreatingCustomer.v1;

public record CreateCustomerCommand(
    Guid CustomerId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email) : ICommand;
