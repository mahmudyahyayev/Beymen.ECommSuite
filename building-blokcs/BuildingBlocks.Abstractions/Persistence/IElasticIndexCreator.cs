namespace BuildingBlocks.Abstractions.Persistence;

public interface IElasticIndexCreator
{
    int OrderNumber { get; }
    Task CreateIndexAsync(CancellationToken cancellationToken);
}
