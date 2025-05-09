using System.Data;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Abstractions.Persistence.Efcore
{
    public interface IDbContext : ITxDbContextExecution, IRetryDbContextExecution
    {
        DbSet<TEntity> Set<TEntity>()
            where TEntity : class;

        Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken);
        Task CommitTransactionAsync(CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
