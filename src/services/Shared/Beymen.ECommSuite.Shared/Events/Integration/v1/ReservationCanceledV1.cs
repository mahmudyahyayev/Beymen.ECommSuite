using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Core.Messaging;

namespace Beymen.ECommSuite.Shared.Events.Integration.v1;

public record ReservationCanceledV1(
    Guid ReservationId, 
    Guid OrderId,
    Guid CustomerId, 
    string Message,
    string MessageKey,
    int Priority) : IntegrationEvent(MessageKey, Priority, MessageSendMode.Publish);