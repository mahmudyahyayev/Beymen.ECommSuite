using BuildingBlocks.Abstractions.CQRS.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notification.Application.Services;
using Notification.Domain;
namespace Notification.Application.Features.Notifications.CreatingNotification.v1.Internals.Internals;

public class SendEmailCommandHandler : ICommandHandler<SendEmailCommand>
{
    private readonly ILogger<SendEmailCommandHandler> _logger;
    private readonly IApplicationDbContext _context;
    private readonly IEmailSenderService _emailSenderService;

    public SendEmailCommandHandler(ILogger<SendEmailCommandHandler> logger,
        IApplicationDbContext context,
        IEmailSenderService emailSenderService)
    {
        _logger = logger;
        _context = context;
        _emailSenderService = emailSenderService;
    }

    public async Task<Unit> Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(u => u.Id == request.CustomerId, cancellationToken);

        ArgumentNullException.ThrowIfNull(customer, nameof(customer));

        if (!customer.IsActive)
            ArgumentNullException.ThrowIfNull(customer, nameof(customer));

        var notification = await _context.Notifications.SingleOrDefaultAsync(u => u.Id == request.NotificationId, cancellationToken);

        ArgumentNullException.ThrowIfNull(notification, nameof(notification));

        var result = await _emailSenderService.SendEmailAsync(customer.Email, request.Message, cancellationToken);

        if (result.success)
        {
            notification.MarkAsSent();
            notification.SetRecipient(customer.Email);
        }
        else
        {
            notification.MarkAsFailed(result.message);
        }
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}