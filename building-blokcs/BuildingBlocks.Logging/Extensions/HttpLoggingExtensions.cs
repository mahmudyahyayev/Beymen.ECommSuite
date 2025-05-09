using BuildingBlocks.Logging.Interceptor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Logging.Extensions;

public static class HttpLoggingExtensions
{
    public static WebApplicationBuilder AddCustomHttpLogging(
        this WebApplicationBuilder builder,
        ISet<string> unredactedHeaders = null,
        bool blockDefaultSwaggerPayloads = true)
    {
        builder.Services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.RequestHeaders |
                                    HttpLoggingFields.ResponseHeaders |
                                    HttpLoggingFields.RequestBody |
                                    HttpLoggingFields.ResponseBody |
                                    HttpLoggingFields.RequestPath |
                                    HttpLoggingFields.RequestQuery |
                                    HttpLoggingFields.Duration;
        
            options.RequestBodyLogLimit = 4096;
            options.ResponseBodyLogLimit = 4096;
            options.CombineLogs = true;
            if (unredactedHeaders == null) return;
            foreach (var header in unredactedHeaders)
            {
                options.RequestHeaders.Add(header);
            }
        });

        if (blockDefaultSwaggerPayloads)
            builder.Services.AddHttpLoggingInterceptor<LoggingInterceptor>();

        return builder;
    }
}
