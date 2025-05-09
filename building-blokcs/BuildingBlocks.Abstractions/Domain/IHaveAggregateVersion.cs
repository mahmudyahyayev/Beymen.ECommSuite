namespace BuildingBlocks.Abstractions.Domain
{
    public interface IHaveAggregateVersion
    {
        long OriginalVersion { get; }
    }
}
