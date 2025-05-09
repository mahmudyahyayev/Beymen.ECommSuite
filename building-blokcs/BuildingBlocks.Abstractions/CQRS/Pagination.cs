namespace BuildingBlocks.Abstractions.CQRS
{
    public sealed record Pagination
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
