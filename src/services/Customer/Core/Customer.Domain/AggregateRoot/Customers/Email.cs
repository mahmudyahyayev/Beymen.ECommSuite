using BuildingBlocks.Core.Domain;
using FluentValidation;

namespace Customer.Domain.AggregateRoot.Customers
{
    public class Email : ValueObject
    {
        private string _value;
        public string Value => _value;
        private Email() { }
        public string EmailLower => _value.ToLower();
        public static Email Of(string value)
        {
            new EmailValidator().ValidateAndThrow(value);

            Email email = new()
            {
                _value = value
            };
            return email;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _value;
        }

        private sealed class EmailValidator : AbstractValidator<string>
        {
            public EmailValidator()
            {
                RuleFor(email => email).NotEmpty();
                RuleFor(email => email).EmailAddress();
            }
        }
    }
}
