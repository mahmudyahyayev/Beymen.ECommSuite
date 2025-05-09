namespace BuildingBlocks.Abstractions.Domain
{
    public interface IHaveAudit<T> : IHaveCreator<T>
    {
        DateTime? LastModified { get; }
        T? LastModifiedBy { get; }
    }
}
