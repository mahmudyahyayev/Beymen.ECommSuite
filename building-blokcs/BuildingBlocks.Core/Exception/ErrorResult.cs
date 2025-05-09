namespace BuildingBlocks.Core.Exception;

public class ErrorResult
{
    public string ExceptionId { get; set; }

    public string Message { get; set; }

    public ErrorResult(string exceptionId, string message)
    {
        ExceptionId = exceptionId;
        Message = message;
    }
}
