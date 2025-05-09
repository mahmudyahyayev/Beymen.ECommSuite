namespace BuildingBlocks.Abstractions.CQRS.Queries
{
    public interface ICacheQuery<out TResponse> : IQuery<TResponse>
      where TResponse : notnull
    {
    }
}
