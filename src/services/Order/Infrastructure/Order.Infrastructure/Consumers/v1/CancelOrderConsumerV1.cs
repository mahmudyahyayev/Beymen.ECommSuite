using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beymen.ECommSuite.Shared.Events.Integration.v1;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Order.Infrastructure.Consumers.v1;

public class CancelOrderConsumerV1 : IConsumer<ReservationCanceledV1>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly ILogger<CancelOrderConsumerV1> _logger;
    private readonly IMessagePersistenceService _messagePersistenceService;

    public CancelOrderConsumerV1(
        ICommandProcessor commandProcessor,
        ILogger<CancelOrderConsumerV1> logger,
        IMessagePersistenceService messagePersistenceService)
    {
        _commandProcessor = commandProcessor;
        _logger = logger;
        _messagePersistenceService = messagePersistenceService;
    }

    public async Task Consume(ConsumeContext<ReservationCanceledV1> context)
    {
        _logger.LogInformation("Start to consuming cancel order");

        var wrappedIntegrationEvent = context.Message;

        await _messagePersistenceService
            .AddReceivedMessageAsync(
                messageEnvelope: new MessageEnvelope(wrappedIntegrationEvent, new Dictionary<string, object?>()),
                cancellationToken: context.CancellationToken);

        _logger.LogInformation("End to consuming cancel order");
    }
}
