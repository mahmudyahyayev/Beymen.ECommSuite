using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Order.Application.Features.Addresses.CreatingAddress.v1.Internals;

public record CreateAddress(
     Guid AddressId,
    Guid CustomerId,
    int Type,
    string FullAddress) : ITxCreateCommand<bool>;

