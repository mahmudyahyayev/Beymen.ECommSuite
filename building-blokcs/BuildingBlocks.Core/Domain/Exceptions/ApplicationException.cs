using BuildingBlocks.Core.Exception;

namespace BuildingBlocks.Core.Domain.Exceptions;

[Serializable]
public abstract class ApplicationException : BaseException
{
    public ApplicationException(string message, string exceptionId, int statusCode)
        : base(message, exceptionId, statusCode) { }
}
