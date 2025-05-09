using System.Reflection;
using BuildingBlocks.Core.Persistence.Efcore;
using BuildingBlocks.Core.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Order.Domain;
using Order.Domain.AggregateRoot.Orders;
using Order.Domain.Projections;

namespace Order.Infrastructure.Context;

public class ApplicationDbContext : EfDbContextBase, IApplicationDbContext
{
    public const string DefaultSchema = "public";
    public const string OrderSchema = "order";
    public const string AddressSchema = "address";
    public const string ProductSchema = "product";
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension(EfConstants.UuidGenerator);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    public DatabaseFacade Connection => this.Database;


    public DbSet<Domain.AggregateRoot.Orders.Order> Orders => Set<Domain.AggregateRoot.Orders.Order>();


    public DbSet<OrderItem> OrderItems => Set<OrderItem>();


    public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();


    public DbSet<AddressReadModel> Addresses => Set<AddressReadModel>();


    public DbSet<ProductReadModel> Products => Set<ProductReadModel>();

}
