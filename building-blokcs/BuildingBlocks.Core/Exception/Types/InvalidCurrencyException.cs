using BuildingBlocks.Core.Domain.Exceptions;

namespace BuildingBlocks.Core.Exception.Types;

[Serializable]
public class InvalidCurrencyException : DomainException
{
    public InvalidCurrencyException(string currency)
        : base($"Currency: '{currency}' is invalid.", "SYS_1000", 400)
    {
        Data.Add("currency", currency);
    }
}
