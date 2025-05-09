using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Core.Messaging;

namespace Beymen.ECommSuite.Shared.Events.Integration.v1;

public record CustomerMainAttributesChangedV1(
     Guid CustomerId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    string MessageKey,
    int Priority) : IntegrationEvent(MessageKey, Priority, MessageSendMode.Publish);