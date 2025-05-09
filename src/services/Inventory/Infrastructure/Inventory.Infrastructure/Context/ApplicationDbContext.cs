using System.Reflection;
using BuildingBlocks.Core.Persistence.Efcore;
using BuildingBlocks.Core.Persistence.EfCore;
using Inventory.Domain;
using Inventory.Domain.AggregateRoot.Products;
using Inventory.Domain.AggregateRoot.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Inventory.Infrastructure.Context;
public class ApplicationDbContext : EfDbContextBase, IApplicationDbContext
{
    public const string DefaultSchema = "public";
    public const string ProductSchema = "product";
    public const string RezervationSchema = "rezervation";
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension(EfConstants.UuidGenerator);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    public DatabaseFacade Connection => this.Database;

    public DbSet<Product> Products => Set<Product>();

    public DbSet<ProductStatus> ProductStatusues => Set<ProductStatus>();

    public DbSet<Reservation> Reservations => Set<Reservation>();

    public DbSet<ReservationItem> ReservationItems => Set<ReservationItem>();

    public DbSet<ReservationStatus> ReservationStatuses => Set<ReservationStatus>();
}


