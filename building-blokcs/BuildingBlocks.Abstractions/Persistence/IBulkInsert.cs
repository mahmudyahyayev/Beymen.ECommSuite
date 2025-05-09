namespace BuildingBlocks.Abstractions.Persistence
{
    public interface ISeed
    {
        int OrderNumber { get; }
    }

    public interface IBulkInsert : ISeed
    {
        Task BulkInsertAllAsync(CancellationToken cancellationToken);
    }

    public interface IDataSeeder : ISeed
    {
        Task SeedAllAsync(CancellationToken cancellationToken);
    }
}
