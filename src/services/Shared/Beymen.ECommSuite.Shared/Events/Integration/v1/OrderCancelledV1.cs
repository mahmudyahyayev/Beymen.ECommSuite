using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Core.Messaging;

namespace Beymen.ECommSuite.Shared.Events.Integration.v1;

public record OrderCancelledV1(
    Guid OrderId,
    Guid CustomerId,
    string Reason,
    string MessageKey,
    int Priority) : IntegrationEvent(MessageKey, Priority, MessageSendMode.Publish);
