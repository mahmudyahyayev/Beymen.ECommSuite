using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;

namespace Customer.Infrastructure.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddRateLimiter(this WebApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(limiterOpt =>
        {
            limiterOpt.AddFixedWindowLimiter("SecurityPolicy.RateLimiting.FixedWindow", config =>
            {
                config.PermitLimit = 10;
                config.Window = TimeSpan.FromSeconds(10);
                config.QueueLimit = 0;
            });
            limiterOpt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });
        return builder;
    }
}
