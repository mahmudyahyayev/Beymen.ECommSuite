using BuildingBlocks.Abstractions.CQRS;

namespace BuildingBlocks.Core.CQRS.Queries;

public record ListResultModel<T>(List<T> Items, long TotalItems, int Page, int PageSize)
    where T : notnull
{
    public static ListResultModel<T> Empty => new(Enumerable.Empty<T>().ToList(), 0, 0, 0);

    public static ListResultModel<T> Create(List<T> items, long totalItems = 0, int page = 1, int pageSize = 10)
    {
        return new ListResultModel<T>(items, totalItems, page, pageSize);
    }

    public ListResultModel<U> Map<U>(Func<T, U> map)
    {
        return ListResultModel<U>.Create(Items.Select(map).ToList(), TotalItems, Page, PageSize);
    }
}

public record PagedList<T> where T : class
{
    public PagedList() { }
    public PagedList(Pagination pagination)
    {
        Page = pagination.Page;
        PageSize = pagination.PageSize;
        Records = [];
    }
    public PagedList(List<T> records, int page, int pageSize, int totalRecords)
    {
        Page = page;
        PageSize = pageSize;
        TotalRecords = totalRecords;
        Records = records;
    }

    public List<T> Records { get; set; } = default!;
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);
    public List<T> SelectedRows { get; set; }

    public bool HasAnyRecord() => Records.Count != 0;
}
