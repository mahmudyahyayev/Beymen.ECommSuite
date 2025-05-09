namespace BuildingBlocks.Abstractions.Persistence
{
    public interface ITransactionAble
    {
        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync(CancellationToken cancellationToken);
    }
}
