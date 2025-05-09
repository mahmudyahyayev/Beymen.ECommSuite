using BuildingBlocks.Core.Domain.Exceptions;

namespace BuildingBlocks.Core.Exception.Types;

[Serializable]
public class InvalidPhoneNumberException : DomainException
{
    public InvalidPhoneNumberException(string phoneNumber)
        : base($"PhoneNumber: '{phoneNumber}' is invalid.", "SYS_1004", 400)
    {
        Data.Add("phoneNumber", phoneNumber);
    }
}
