using System.Text.Json.Serialization;

namespace BuildingBlocks.Core.Exception;

public class ErrorModel
{
    [JsonIgnore]
    public int StatusCode { get; set; }
    public string ExceptionId { get; set; }
    public string Message { get; set; }


    public ErrorModel(string message, string exceptionId, int statusCode)
    {
        StatusCode = statusCode;
        Message = message;
        ExceptionId = exceptionId;
    }
}
