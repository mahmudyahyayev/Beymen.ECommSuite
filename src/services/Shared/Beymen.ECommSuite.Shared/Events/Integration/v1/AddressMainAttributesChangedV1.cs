using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Core.Messaging;

namespace Beymen.ECommSuite.Shared.Events.Integration.v1;

public record AddressMainAttributesChangedV1(
    Guid AddressId,
    string FullAddress,
    string MessageKey,
    int Priority) : IntegrationEvent(MessageKey, Priority, MessageSendMode.Publish);
