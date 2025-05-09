using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Core.Messaging;

namespace Beymen.ECommSuite.Shared.Events.Integration.v1;

public record OrderCreatedV1(
    Guid OrderId,
    Guid CustomerId,
    IReadOnlyList<OrderCreatedItem> Items,
    string MessageKey,
    int Priority) : IntegrationEvent(MessageKey, Priority, MessageSendMode.Publish);

public class OrderCreatedItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}


