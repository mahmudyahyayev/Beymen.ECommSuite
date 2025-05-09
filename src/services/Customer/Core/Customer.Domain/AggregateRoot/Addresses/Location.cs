using BuildingBlocks.Core.Domain;
using FluentValidation;

namespace Customer.Domain.AggregateRoot.Addresses
{
    public class Location : ValueObject
    {
        private string _value;
        public string Value => _value;
        private Location() { }

        public static Location Of(string name)
        {
            new LocationValidator()
                .ValidateAndThrow(name);

            return new Location
            {
                _value = name
            };
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _value;
        }

        private sealed class LocationValidator : AbstractValidator<string>
        {
            public LocationValidator()
            {
                RuleFor(value => value)
                    .NotNull()
                    .MaximumLength(250);
            }
        }
    }
}
