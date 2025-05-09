using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Customer.Application.Features.Addresses.CreatingAddress.v1;

public record CreateAddress(
    Guid CustomerId,
    int TypeId,
    string Country,
    string City,
    string District,
    string Street,
    string ZipCode,
    string BuildingNo,
    string ApartmentNo,
    string Floor,
    string Description) : ITxCreateCommand<bool>;
