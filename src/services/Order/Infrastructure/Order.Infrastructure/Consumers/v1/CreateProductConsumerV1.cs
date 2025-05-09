using Beymen.ECommSuite.Shared.Events.Integration.v1;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Order.Infrastructure.Consumers.v1;

public class CreateProductConsumerV1 : IConsumer<ProductCreatedV1>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly ILogger<CreateProductConsumerV1> _logger;
    private readonly IMessagePersistenceService _messagePersistenceService;

    public CreateProductConsumerV1(
        ICommandProcessor commandProcessor,
        ILogger<CreateProductConsumerV1> logger,
        IMessagePersistenceService messagePersistenceService)
    {
        _commandProcessor = commandProcessor;
        _logger = logger;
        _messagePersistenceService = messagePersistenceService;
    }

    public async Task Consume(ConsumeContext<ProductCreatedV1> context)
    {
        _logger.LogInformation("Start to consuming create product");

        var wrappedIntegrationEvent = context.Message;

        await _messagePersistenceService
            .AddReceivedMessageAsync(
                messageEnvelope: new MessageEnvelope(wrappedIntegrationEvent, new Dictionary<string, object?>()),
                cancellationToken: context.CancellationToken);

        _logger.LogInformation("End to consuming create product");
    }
}

