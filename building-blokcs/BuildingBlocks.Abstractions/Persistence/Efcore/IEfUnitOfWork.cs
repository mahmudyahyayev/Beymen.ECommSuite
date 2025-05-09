using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Abstractions.Persistence.Efcore
{
    public interface IEfUnitOfWork : IUnitOfWork, ITransactionAble, ITxDbContextExecution, IRetryDbContextExecution
    {
        T GetRepository<T>() where T : IRepository;
    }
    public interface IEfUnitOfWork<out TContext> : IEfUnitOfWork
        where TContext : DbContext
    {
        TContext DbContext { get; }
    }
}
