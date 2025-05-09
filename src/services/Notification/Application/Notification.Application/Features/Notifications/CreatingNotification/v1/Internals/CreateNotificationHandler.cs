using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Microsoft.Extensions.Logging;
using Notification.Domain;

namespace Notification.Application.Features.Notifications.CreatingNotification.v1.Internals;

public class CreateNotificationHandler : ICommandHandler<CreateNotification, bool>
{
    private readonly ILogger<CreateNotificationHandler> _logger;
    private readonly IApplicationDbContext _context;

    public CreateNotificationHandler(ILogger<CreateNotificationHandler> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<bool> Handle(CreateNotification command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var notification = Domain.AggregateRoot.Notifications.Notification.Create(command.CustomerId, command.Type, command.Message);

        await _context.Notifications.AddAsync(notification, cancellationToken);

        var result = await _context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}
