using BuildingBlocks.Abstractions.Domain;

namespace Customer.Domain.AggregateRoot.Customers;

public record CustomerId : AggregateId
{
    private CustomerId(Guid value)
        : base(value) { }
    public static CustomerId Of(Guid id) => new(id);

    public static implicit operator Guid(CustomerId id) => id.Value;
}
