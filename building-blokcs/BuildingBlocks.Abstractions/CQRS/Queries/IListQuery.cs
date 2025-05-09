namespace BuildingBlocks.Abstractions.CQRS.Queries;

public interface IListQuery<out TResponse> : ILoadOptionsRequest, IQuery<TResponse>
    where TResponse : notnull { }
