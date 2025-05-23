using BuildingBlocks.Abstractions.CQRS.Queries;
using MediatR;

namespace BuildingBlocks.Core.CQRS.Queries;

public class QueryProcessor : IQueryProcessor
{
    private readonly IMediator _mediator;

    public QueryProcessor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
    {
        return _mediator.Send(query, cancellationToken);
    }

    public IAsyncEnumerable<TResponse> SendAsync<TResponse>(
        IStreamQuery<TResponse> query,
        CancellationToken cancellationToken
    )
    {
        return _mediator.CreateStream(query, cancellationToken);
    }
}
