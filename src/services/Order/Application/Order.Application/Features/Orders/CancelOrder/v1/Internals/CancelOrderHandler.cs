using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Order.Domain;

namespace Order.Application.Features.Orders.CancelOrder.v1.Internals;

public class CancelOrderHandler : ICommandHandler<CancelOrder, bool>
{
    private readonly ILogger<CancelOrderHandler> _logger;
    private readonly IApplicationDbContext _context;

    public CancelOrderHandler(ILogger<CancelOrderHandler> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    //inventory servisdeki herhangi bir g=hatadan dolayi siparisin geri cekilmesi
    public async Task<bool> Handle(CancelOrder command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var order = await _context.Orders.SingleOrDefaultAsync(u => u.Id == command.OrderId, cancellationToken);

        ArgumentNullException.ThrowIfNull(order, nameof(order));

        order.Cancel(command.Message);

        var result = await _context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}
