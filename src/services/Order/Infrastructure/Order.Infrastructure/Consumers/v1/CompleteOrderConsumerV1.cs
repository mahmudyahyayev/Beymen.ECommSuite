using Beymen.ECommSuite.Shared.Events.Integration.v1;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Order.Infrastructure.Consumers.v1;

public class CompleteOrderConsumerV1 : IConsumer<ReservationConfirmedV1>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly ILogger<CompleteOrderConsumerV1> _logger;
    private readonly IMessagePersistenceService _messagePersistenceService;

    public CompleteOrderConsumerV1(
        ICommandProcessor commandProcessor,
        ILogger<CompleteOrderConsumerV1> logger,
        IMessagePersistenceService messagePersistenceService)
    {
        _commandProcessor = commandProcessor;
        _logger = logger;
        _messagePersistenceService = messagePersistenceService;
    }

    public async Task Consume(ConsumeContext<ReservationConfirmedV1> context)
    {
        _logger.LogInformation("Start to consuming complete order");

        var wrappedIntegrationEvent = context.Message;

        await _messagePersistenceService
            .AddReceivedMessageAsync(
                messageEnvelope: new MessageEnvelope(wrappedIntegrationEvent, new Dictionary<string, object?>()),
                cancellationToken: context.CancellationToken);

        _logger.LogInformation("End to consuming complete order");
    }
}
