using BuildingBlocks.Core.Domain;

namespace Inventory.Domain.AggregateRoot.Reservations;

public class ReservationStatus : Enumeration
{
    public static ReservationStatus Pending = new(1, nameof(Pending).ToLowerInvariant());
    public static ReservationStatus Confirmed = new(2, nameof(Confirmed).ToLowerInvariant());
    public static ReservationStatus Cancelled = new(3, nameof(Cancelled).ToLowerInvariant());
    public ReservationStatus(int id, string name) : base(id, name)
    {
    }

    public static IEnumerable<ReservationStatus> List() => new[] { Pending, Confirmed, Cancelled };

    public static ReservationStatus FromName(string name)
    {
        var state = List()
            .SingleOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));

        if (state is null)
        {
            ArgumentNullException.ThrowIfNull(nameof(state));
        }

        return state;
    }

    public static ReservationStatus From(int id)
    {
        var state = List().SingleOrDefault(x => x.Id == id);

        if (state is null)
            ArgumentNullException.ThrowIfNull(nameof(state));

        return state;
    }
}