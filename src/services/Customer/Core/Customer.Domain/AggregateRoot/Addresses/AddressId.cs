using BuildingBlocks.Abstractions.Domain;

namespace Customer.Domain.AggregateRoot.Addresses;
public record AddressId : AggregateId
{
    private AddressId(Guid value)
        : base(value) { }
    public static AddressId Of(Guid id) => new(id);

    public static implicit operator Guid(AddressId id) => id.Value;
}
