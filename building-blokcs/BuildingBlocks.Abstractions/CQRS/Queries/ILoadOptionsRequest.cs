namespace BuildingBlocks.Abstractions.CQRS.Queries;

public interface ILoadOptionsRequest
{
    string? SearchValue { get; set; }
    List<string>? SelectedRowKeys { get; set; }
    List<Sorting>? Sortings { get; set; }
    List<LoadOptionFilter>? Filters { get; set; }
    Pagination PageOption { get; set; }
}
