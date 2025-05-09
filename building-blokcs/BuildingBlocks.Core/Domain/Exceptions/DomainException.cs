using BuildingBlocks.Core.Exception;

namespace BuildingBlocks.Core.Domain.Exceptions;

[Serializable]
public abstract class DomainException : BaseException
{
    public DomainException(string message, string exceptionId, int statusCode)
        : base(message, exceptionId, statusCode) { }
}
