using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Customer.Domain;
using Microsoft.Extensions.Logging;

namespace Customer.Application.Features.Customers.CreatingCustomer.v1;
public class CreateCustomerHandler : ICommandHandler<CreateCustomer, bool>
{
    private readonly ILogger<CreateCustomerHandler> _logger;
    private readonly IApplicationDbContext _context;

    public CreateCustomerHandler(ILogger<CreateCustomerHandler> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<bool> Handle(CreateCustomer command,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var customer = Domain.AggregateRoot.Customers.Customer.Create(command.FirstName, command.LastName, command.PhoneNumber, command.Email);

        await _context.Customers.AddAsync(customer, cancellationToken);

        var result = await _context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}
