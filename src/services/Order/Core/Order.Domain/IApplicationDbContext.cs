using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Order.Domain.AggregateRoot.Orders;
using Order.Domain.Projections;

namespace Order.Domain;

public interface IApplicationDbContext
{
    DbSet<Order.Domain.AggregateRoot.Orders.Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<OrderStatus> OrderStatuses { get; }
    DbSet<AddressReadModel> Addresses { get; }
    DbSet<ProductReadModel> Products { get; }
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}


