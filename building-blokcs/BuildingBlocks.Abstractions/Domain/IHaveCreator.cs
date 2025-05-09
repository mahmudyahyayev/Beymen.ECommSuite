namespace BuildingBlocks.Abstractions.Domain
{
    public interface IHaveCreator<T>
    {
        DateTime Created { get; }
        T? CreatedBy { get; }
    }
}
