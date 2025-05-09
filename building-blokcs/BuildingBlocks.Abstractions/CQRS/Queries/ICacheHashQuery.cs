namespace BuildingBlocks.Abstractions.CQRS.Queries
{
    public interface ICacheHashQuery<out TResponse> : IQuery<TResponse>
     where TResponse : notnull
    {
    }
}
