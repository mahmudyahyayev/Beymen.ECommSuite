using BuildingBlocks.Abstractions.Domain;

namespace Inventory.Domain.AggregateRoot.Reservations;
public record ReservationId : AggregateId
{
    private ReservationId(Guid value)
        : base(value) { }
    public static ReservationId Of(Guid id) => new(id);

    public static implicit operator Guid(ReservationId id) => id.Value;
}
