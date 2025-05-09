using BuildingBlocks.Abstractions.Domain;

namespace Inventory.Domain.AggregateRoot.Reservations
{
    public record ReservationItemId : EntityId
    {
        private ReservationItemId(Guid value)
            : base(value) { }
        public static ReservationItemId Of(Guid id) => new(id);

        public static implicit operator Guid(ReservationItemId id) => id.Value;
    }
}
