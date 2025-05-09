using Customer.Domain.AggregateRoot.Addresses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Customer.Domain
{
    public interface IApplicationDbContext
    {
        DbSet<Address> Addresses { get; }
        DbSet<AddressType> AddressTypes { get; }
        DbSet<AggregateRoot.Customers.Customer> Customers { get; }
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
