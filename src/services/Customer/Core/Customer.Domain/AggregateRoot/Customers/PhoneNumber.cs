using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Exception;
using BuildingBlocks.Core.Exception.Types;

namespace Customer.Domain.AggregateRoot.Customers
{
    public class PhoneNumber : ValueObject
    {
        private string _value;
        public string Value => _value;
        private PhoneNumber() { }

        public static PhoneNumber Of(string value)
        {
            Guard.Against.InvalidPhoneNumber(value, new InvalidPhoneNumberException(value));

            PhoneNumber phoneNumber = new()
            {
                _value = value
            };

            return phoneNumber;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _value;
        }
    }
}
