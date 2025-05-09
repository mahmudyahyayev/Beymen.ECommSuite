namespace BuildingBlocks.Abstractions.CQRS;

public record DateRangeFilter
{
    public DateRangeFilter(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

