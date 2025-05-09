using BuildingBlocks.Core.Domain;
using FluentValidation;

namespace Customer.Domain.AggregateRoot.Addresses;

public class ZipCode : ValueObject
{
    private string _value;
    public string Value => _value;

    private ZipCode() { }

    public static ZipCode Of(string zipCode)
    {
        new ZipCodeValidator()
            .ValidateAndThrow(zipCode);

        return new ZipCode
        {
            _value = zipCode
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }

    private sealed class ZipCodeValidator : AbstractValidator<string>
    {
        public ZipCodeValidator()
        {
            RuleFor(value => value)
                .NotNull()
                .Matches(@"^\d{5}(-\d{4})?$")
                .WithMessage("ZipCode must be a valid postal code format.");
        }
    }
}
