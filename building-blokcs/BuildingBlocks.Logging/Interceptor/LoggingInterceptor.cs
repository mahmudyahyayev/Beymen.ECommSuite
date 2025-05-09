using Microsoft.AspNetCore.HttpLogging;

namespace BuildingBlocks.Logging.Interceptor;

// PossibleBuildingBlockPart:
public class LoggingInterceptor : IHttpLoggingInterceptor
{
    public ValueTask OnRequestAsync(HttpLoggingInterceptorContext logContext)
    {
        if (logContext.HttpContext.Request.Path.Value?.Contains("swagger", StringComparison.Ordinal) ?? false)
        {
            logContext.LoggingFields = HttpLoggingFields.None;
        }
        
        return default;
    }

    public ValueTask OnResponseAsync(HttpLoggingInterceptorContext logContext)
    {
        return default;
    }
}