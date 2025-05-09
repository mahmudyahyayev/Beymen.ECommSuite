using BuildingBlocks.Core.Domain.Exceptions;

namespace BuildingBlocks.Core.Exception.Types;


[Serializable]
public class InvalidIpAddressException : DomainException
{
    public InvalidIpAddressException(string ipAddress)
        : base($"IpAddress: '{ipAddress}' is invalid.", "SYS_1003", 400)
    {
        Data.Add("ipAddress", ipAddress);
    }
}
