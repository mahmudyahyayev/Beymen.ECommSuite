using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Inventory.Infrastructure.Shared.ExceptionHandlers;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class ExtensionProblemDetails : ProblemDetails
{
    public bool Success => false;  
    public int StatusCode { get; set; } 
    public object Result => null;
    public required string CorrelationId { get; set; }
    public List<ErrorList> ErrorList { get; set; }
    public ExtensionProblemDetails()
    {
        ErrorList = new List<ErrorList>();
    }
}

public class ErrorList
{
    public string Code { get; set; }
    public string Message { get; set; }
}
