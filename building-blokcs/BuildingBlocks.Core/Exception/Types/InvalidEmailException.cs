using BuildingBlocks.Core.Domain.Exceptions;

namespace BuildingBlocks.Core.Exception.Types;
[Serializable]
public class InvalidEmailException : DomainException
{
    public InvalidEmailException(string email)
        : base($"Email: '{email}' is invalid.", "SYS_1002", 400)
    {
        Data.Add("email", email);
    }
}
