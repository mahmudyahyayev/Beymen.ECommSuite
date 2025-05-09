namespace BuildingBlocks.Abstractions.CQRS
{
    public record Sorting
    {
        public string ColumnName { get; set; }
        public SortingTypes SortingType { get; set; }
    }

    public enum SortingTypes
    {
        Asc = 1,
        Desc = 2
    }
}
