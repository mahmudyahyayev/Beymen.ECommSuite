using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Core.Messaging;

namespace Beymen.ECommSuite.Shared.Events.Integration.v1;

public record ProductCreatedV1(
Guid ProductId, 
string Name,
string Description,
decimal Price, 
int Stock,
int Status,
string MessageKey,
int Priority) : IntegrationEvent(MessageKey, Priority, MessageSendMode.Publish);

