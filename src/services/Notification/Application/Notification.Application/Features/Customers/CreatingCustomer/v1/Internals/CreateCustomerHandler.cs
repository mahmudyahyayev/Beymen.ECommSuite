using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Microsoft.Extensions.Logging;
using Notification.Domain;
using Notification.Domain.Projections;
namespace Notification.Application.Features.Customers.CreatingCustomer.v1.Internals;

public class CreateCustomerHandler : ICommandHandler<CreateCustomer, bool>
{
    private readonly ILogger<CreateCustomerHandler> _logger;
    private readonly IApplicationDbContext _context;

    public CreateCustomerHandler(ILogger<CreateCustomerHandler> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<bool> Handle(CreateCustomer command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var customer = CustomerReadModel.Create(command.CustomerId, command.FirstName, command.LastName, command.PhoneNumber, command.Email);

        await _context.Customers.AddAsync(customer, cancellationToken);

        var result = await _context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}