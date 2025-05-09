using Beymen.ECommSuite.Shared.Events.Integration.v1;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Notification.Infrastructure.Consumers.v1;

public class SendNotificationConsumerV1 : IConsumer<SendNotificationV1>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly ILogger<SendNotificationConsumerV1> _logger;
    private readonly IMessagePersistenceService _messagePersistenceService;

    public SendNotificationConsumerV1(
        ICommandProcessor commandProcessor,
        ILogger<SendNotificationConsumerV1> logger,
        IMessagePersistenceService messagePersistenceService)
    {
        _commandProcessor = commandProcessor;
        _logger = logger;
        _messagePersistenceService = messagePersistenceService;
    }

    public async Task Consume(ConsumeContext<SendNotificationV1> context)
    {
        _logger.LogInformation("Start to consuming send notification");

        var wrappedIntegrationEvent = context.Message;

        await _messagePersistenceService
            .AddReceivedMessageAsync(
                messageEnvelope: new MessageEnvelope(wrappedIntegrationEvent, new Dictionary<string, object?>()),
                cancellationToken: context.CancellationToken);

        _logger.LogInformation("End to consuming send notification");
    }
}
