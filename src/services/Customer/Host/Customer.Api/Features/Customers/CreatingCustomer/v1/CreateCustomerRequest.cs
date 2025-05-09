namespace Customer.Api.Features.Customers.CreatingCustomer.v1
{
    public record CreateCustomerRequest(
        string FirstName,
        string LastName,
        string PhoneNumber,
        string Email);
}