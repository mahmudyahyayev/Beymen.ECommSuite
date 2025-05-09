using BuildingBlocks.Abstractions.CQRS;
using BuildingBlocks.Abstractions.CQRS.Queries;

namespace BuildingBlocks.Core.CQRS.Queries;

public record ListQuery<TResponse> : IListQuery<TResponse>
    where TResponse : notnull
{
    public string? SearchValue { get; set; }
    public List<string>? SelectedRowKeys { get; set; }
    public List<Sorting>? Sortings { get; set; }
    public List<LoadOptionFilter>? Filters { get; set; }
    public Pagination PageOption { get; set; }
}
