using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Core.Messaging;

namespace Beymen.ECommSuite.Shared.Events.Integration.v1;

public record AddressCreatedV1(
    Guid AddressId,
    Guid CustomerId,
    int Type,
    string FullAddress,
    string MessageKey,
    int Priority) : IntegrationEvent(MessageKey, Priority, MessageSendMode.Publish);

