using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Order.Application.Features.Addresses.CreatingAddress.v1;

public record CreateAddressCommand(
     Guid AddressId,
    Guid CustomerId,
    int Type,
    string FullAddress) : ICommand;

