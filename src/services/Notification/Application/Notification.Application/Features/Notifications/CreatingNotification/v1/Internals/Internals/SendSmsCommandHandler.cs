using BuildingBlocks.Abstractions.CQRS.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notification.Application.Services;
using Notification.Domain;
namespace Notification.Application.Features.Notifications.CreatingNotification.v1.Internals.Internals;

public class SendSmsCommandHandler : ICommandHandler<SendSmsCommand>
{
    private readonly ILogger<SendSmsCommandHandler> _logger;
    private readonly IApplicationDbContext _context;
    private readonly ISmsSenderService _smsSenderService;

    public SendSmsCommandHandler(ILogger<SendSmsCommandHandler> logger,
        IApplicationDbContext context,
        ISmsSenderService smsSenderService)
    {
        _logger = logger;
        _context = context;
        _smsSenderService = smsSenderService;
    }

    public async Task<Unit> Handle(SendSmsCommand request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(u => u.Id == request.CustomerId, cancellationToken);

        ArgumentNullException.ThrowIfNull(customer, nameof(customer));

        if (!customer.IsActive)
            ArgumentNullException.ThrowIfNull(customer, nameof(customer));

        var notification = await _context.Notifications.SingleOrDefaultAsync(u => u.Id == request.NotificationId, cancellationToken);

        ArgumentNullException.ThrowIfNull(notification, nameof(notification));

        var result = await _smsSenderService.SendSmsAsync(customer.PhoneNumber, request.Message, cancellationToken);

        if (result.success)
        {
            notification.MarkAsSent();
            notification.SetRecipient(customer.PhoneNumber);
        }
        else
        {
            notification.MarkAsFailed(result.message);
        }
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}