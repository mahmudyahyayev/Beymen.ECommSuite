using System.Reflection;
using BuildingBlocks.Core.Persistence.Efcore;
using BuildingBlocks.Core.Persistence.EfCore;
using Customer.Domain;
using Customer.Domain.AggregateRoot.Addresses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Customer.Infrastructure.Context;

public class ApplicationDbContext : EfDbContextBase, IApplicationDbContext
{
    public const string DefaultSchema = "public";
    public const string AddressSchema = "address";
    public const string CustomerSchema = "customer";
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension(EfConstants.UuidGenerator);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    public DatabaseFacade Connection => this.Database;
    public DbSet<AddressType> AddressTypes => Set<AddressType>();
    public DbSet<Address> Addresses => Set<Address>();
    public  DbSet<Domain.AggregateRoot.Customers.Customer> Customers => Set<Domain.AggregateRoot.Customers.Customer>();
    
}
