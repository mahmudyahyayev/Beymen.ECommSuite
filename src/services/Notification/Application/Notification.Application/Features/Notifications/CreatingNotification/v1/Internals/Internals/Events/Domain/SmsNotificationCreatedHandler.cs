using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Correlation;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.CQRS.Events.Internals;
using Notification.Domain.AggregateRoot.Notifications.Events.Domain;

namespace Notification.Application.Features.Notifications.CreatingNotification.v1.Internals.Internals.Events.Domain;

//internal async eventhandler
internal class SmsNotificationCreatedHandler : IDomainEventHandler<SmsNotificationCreated>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly ICorrelationService _correlationService;
    public SmsNotificationCreatedHandler(ICommandProcessor commandProcessor, ICorrelationService correlationService)
    {
        _commandProcessor = commandProcessor;
        _correlationService = correlationService;
    }
    public async Task Handle(SmsNotificationCreated notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));

        var internalCommand = new SendSmsCommand(
            notification.NotificationId,
            notification.CustomerId,
            notification.Message,
            notification.MessageKey,
            notification.Priority);

        await _commandProcessor.ScheduleAsync(internalCommand, _correlationService.CorrelationId, cancellationToken);
    }
}

