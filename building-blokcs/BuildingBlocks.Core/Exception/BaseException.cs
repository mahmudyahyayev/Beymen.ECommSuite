namespace BuildingBlocks.Core.Exception;

[Serializable]
public abstract class BaseException : System.Exception
{
    public ErrorModel ErrorModel { get; private set; }

    protected BaseException(string message, string exceptionId, int statusCode) : base(message)
    {
        ErrorModel = new ErrorModel(message, exceptionId, statusCode);
    }
}