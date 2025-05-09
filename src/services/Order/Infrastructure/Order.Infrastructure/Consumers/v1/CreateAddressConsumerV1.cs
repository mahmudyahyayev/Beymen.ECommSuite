using Beymen.ECommSuite.Shared.Events.Integration.v1;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Order.Infrastructure.Consumers.v1;

public class CreateAddressConsumerV1 : IConsumer<AddressCreatedV1>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly ILogger<CreateAddressConsumerV1> _logger;
    private readonly IMessagePersistenceService _messagePersistenceService;

    public CreateAddressConsumerV1(
        ICommandProcessor commandProcessor,
        ILogger<CreateAddressConsumerV1> logger,
        IMessagePersistenceService messagePersistenceService)
    {
        _commandProcessor = commandProcessor;
        _logger = logger;
        _messagePersistenceService = messagePersistenceService;
    }

    public async Task Consume(ConsumeContext<AddressCreatedV1> context)
    {
        _logger.LogInformation("Start to consuming create address");

        var wrappedIntegrationEvent = context.Message;

        await _messagePersistenceService
            .AddReceivedMessageAsync(
                messageEnvelope: new MessageEnvelope(wrappedIntegrationEvent, new Dictionary<string, object?>()),
                cancellationToken: context.CancellationToken);

        _logger.LogInformation("End to consuming create address");
    }
}
