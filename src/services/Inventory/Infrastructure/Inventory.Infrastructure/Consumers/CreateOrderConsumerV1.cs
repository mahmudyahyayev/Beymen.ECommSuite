using Beymen.ECommSuite.Shared.Events.Integration.v1;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Consumers;


public class CreateOrderConsumerV1 : IConsumer<OrderCreatedV1>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly ILogger<CreateOrderConsumerV1> _logger;
    private readonly IMessagePersistenceService _messagePersistenceService;

    public CreateOrderConsumerV1(
        ICommandProcessor commandProcessor,
        ILogger<CreateOrderConsumerV1> logger,
        IMessagePersistenceService messagePersistenceService)
    {
        _commandProcessor = commandProcessor;
        _logger = logger;
        _messagePersistenceService = messagePersistenceService;
    }

    public async Task Consume(ConsumeContext<OrderCreatedV1> context)
    {
        _logger.LogInformation("Start to consuming create order");

        var wrappedIntegrationEvent = context.Message;

        await _messagePersistenceService
            .AddReceivedMessageAsync(
                messageEnvelope: new MessageEnvelope(wrappedIntegrationEvent, new Dictionary<string, object?>()),
                cancellationToken: context.CancellationToken);

        _logger.LogInformation("End to consuming create order");
    }
}

