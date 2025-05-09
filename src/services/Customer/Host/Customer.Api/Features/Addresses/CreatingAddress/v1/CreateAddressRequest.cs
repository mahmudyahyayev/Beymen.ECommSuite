namespace Customer.Api.Features.Addresses.CreatingAddress.v1;

public record CreateAddressRequest(
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
    string Description);