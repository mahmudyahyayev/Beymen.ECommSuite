using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Core.Domain;
using Inventory.Domain.AggregateRoot.Products;
using Inventory.Domain.AggregateRoot.Reservations.Events.Domain;

namespace Inventory.Domain.AggregateRoot.Reservations;

public class Reservation :Aggregate<ReservationId, string>,
    IHaveAudit<string>,
    IHaveIdentity<ReservationId>
{
    private CustomerId _customerId;
    public CustomerId CustomerId => _customerId;

    private OrderId _orderId;
    public OrderId OrderId => _orderId;

    private List<ReservationItem> _items = new();
    public IReadOnlyList<ReservationItem> Items => _items;

    private int _status;
    public ReservationStatus Status { get; private set; }

    private string _errorMessage;
    public string ErrorMessage => _errorMessage;

    /// <summary>
    /// Auditable
    /// </summary>
    private string _lastModifiedBy;
    public string LastModifiedBy => _lastModifiedBy;
    private DateTime? _lastModified;
    public DateTime? LastModified => _lastModified;

    private Reservation()
    {
        _items = new List<ReservationItem>();
    }

    public static Reservation Create(Guid customerId, Guid orderId)
    {
        var reservation = new Reservation
        {
            Id = ReservationId.Of(Guid.NewGuid()),
            _customerId = CustomerId.Of(customerId),
            _orderId  = OrderId.Of(orderId),
            _status = ReservationStatus.Pending.Id,
        };

        return reservation;
    }

    public void AddItem(Guid productId, int quantity)
    {
        var item = ReservationItem.Create(productId, quantity);
        _items.Add(item);
    }

    public void ConfirmReservation()
    {
        _status = ReservationStatus.Confirmed.Id;
        AddDomainEvents(new ReservationConfirmed(Id, _orderId.Value, _customerId.Value));
    }

    public void CancelReservation(string errorMessage)
    {
        _status = ReservationStatus.Cancelled.Id;
        _errorMessage = errorMessage;
        AddDomainEvents(new ReservationCanceled(Id, _orderId.Value, _customerId.Value, errorMessage));
    }
}
