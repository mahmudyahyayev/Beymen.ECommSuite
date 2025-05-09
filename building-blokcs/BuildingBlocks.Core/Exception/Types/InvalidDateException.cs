using BuildingBlocks.Core.Domain.Exceptions;

namespace BuildingBlocks.Core.Exception.Types;
[Serializable]
public class InvalidDateException : DomainException
{
    public InvalidDateException(DateTime dateTime)
        : base($"Date: '{dateTime}' is invalid.", "SYS_1001", 400)
    {
        Data.Add("dateTime", dateTime);
    }
}
