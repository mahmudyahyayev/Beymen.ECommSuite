using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Correlation;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.CQRS.Events.Internals;
using Notification.Domain.AggregateRoot.Notifications.Events.Domain;

namespace Notification.Application.Features.Notifications.CreatingNotification.v1.Internals.Internals.Events.Domain;


//internal async eventhandler
internal class EmailNotificationCreatedHandler : IDomainEventHandler<EmailNotificationCreated>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly ICorrelationService _correlationService;
    public EmailNotificationCreatedHandler(ICommandProcessor commandProcessor, ICorrelationService correlationService)
    {
        _commandProcessor = commandProcessor;
        _correlationService = correlationService;
    }
    public async Task Handle(EmailNotificationCreated notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));

        var internalCommand = new SendEmailCommand(
            notification.NotificationId,
            notification.CustomerId,
            notification.Message,
            notification.MessageKey,
            notification.Priority);

        await _commandProcessor.ScheduleAsync(internalCommand, _correlationService.CorrelationId, cancellationToken);
    }
}