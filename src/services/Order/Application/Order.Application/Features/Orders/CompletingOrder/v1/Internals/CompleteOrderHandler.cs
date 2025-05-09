using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Order.Domain;

namespace Order.Application.Features.Orders.CompletingOrder.v1.Internals;

public class CompleteOrderHandler : ICommandHandler<CompleteOrder, bool>
{
    private readonly ILogger<CompleteOrderHandler> _logger;
    private readonly IApplicationDbContext _context;

    public CompleteOrderHandler(ILogger<CompleteOrderHandler> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    //inventory servisdeki herhangi bir g=hatadan dolayi siparisin geri cekilmesi
    public async Task<bool> Handle(CompleteOrder command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var order = await _context.Orders.SingleOrDefaultAsync(u => u.Id == command.OrderId, cancellationToken);

        ArgumentNullException.ThrowIfNull(order, nameof(order));

        order.MarkAsPaid();

        var result = await _context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}
