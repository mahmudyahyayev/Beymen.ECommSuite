using Inventory.Domain.AggregateRoot.Products;
using Inventory.Domain.AggregateRoot.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Inventory.Domain;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<ProductStatus> ProductStatusues { get; }
    DbSet<Reservation> Reservations { get; }
    DbSet<ReservationItem> ReservationItems { get; }
    DbSet<ReservationStatus> ReservationStatuses { get; }
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

