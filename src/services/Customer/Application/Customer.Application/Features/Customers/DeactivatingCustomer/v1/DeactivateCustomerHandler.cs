using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Customer.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Customer.Application.Features.Customers.DeactivatingCustomer.v1;

public class DeactivateCustomerHandler : ICommandHandler<DeactivateCustomer, bool>
{
    private readonly ILogger<DeactivateCustomerHandler> _logger;
    private readonly IApplicationDbContext _context;

    public DeactivateCustomerHandler(ILogger<DeactivateCustomerHandler> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<bool> Handle(DeactivateCustomer command,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var customer = await _context.Customers.SingleOrDefaultAsync(u => u.Id == command.Id, cancellationToken);

        ArgumentNullException.ThrowIfNull(customer, nameof(customer));

        customer.Deactivate();

        var result = await _context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}

