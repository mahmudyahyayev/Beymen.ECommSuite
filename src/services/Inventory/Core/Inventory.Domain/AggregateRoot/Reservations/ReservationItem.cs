using BuildingBlocks.Core.Domain;

namespace Inventory.Domain.AggregateRoot.Reservations;
public class ReservationItem : Entity<ReservationItemId>
{
    private Guid _productId;
    public Guid ProductId => _productId;

    private int _quantity;
    public int Quantity => _quantity;

    private ReservationId _reservationId;
    public ReservationId ReservationId => _reservationId;

    private Reservation _reservation;
    public Reservation Reservation => _reservation;

    private ReservationItem() { }

    public static ReservationItem Create(Guid productId, int quantity)
    {
        var item = new ReservationItem
        {
            Id = ReservationItemId.Of(Guid.NewGuid()),
            _productId = productId,
            _quantity = quantity
        };

        return item;
    }
}
