namespace BuildingBlocks.Abstractions.CQRS.Queries
{
    public interface IItemQuery<TId, out TResponse> : IQuery<TResponse>
        where TId : struct
        where TResponse : notnull
    {
        public IList<string> Includes { get; }
        public TId Id { get; }
    }
}
