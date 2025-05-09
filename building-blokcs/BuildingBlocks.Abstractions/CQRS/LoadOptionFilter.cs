namespace BuildingBlocks.Abstractions.CQRS;

public record LoadOptionFilter
{
    public FilterTypes FilterType { get; set; }
    public OperatorTypes? Type { get; set; }
    public ConditionTypes? EqualType { get; set; }
    public string? ColumnName { get; set; }
    public object? Value { get; set; }
    public IEnumerable<LoadOptionFilter>? Filters { get; set; } = default!;
}

public enum ConditionTypes
{
    Contains = 1,
    StartsWith,
    EndsWith,
    GreaterThan,
    LessThan,
    GreaterEqual,
    LessEqual,
    Equals,
    NotEquals,
    In
}

public enum FilterTypes
{
    Group = 1,
    Constraint
}
public enum OperatorTypes
{
    And = 1,
    Or
}
