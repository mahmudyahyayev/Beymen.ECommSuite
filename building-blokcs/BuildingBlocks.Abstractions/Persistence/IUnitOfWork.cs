namespace BuildingBlocks.Abstractions.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitAsync(CancellationToken cancellationToken);
    }

    public interface IUnitOfWork<out TContext> : IUnitOfWork
        where TContext : class
    {
        TContext Context { get; }
    }
}
