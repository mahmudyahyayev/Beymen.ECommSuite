namespace Notification.Api.Extensions.ApplicationBuilderExtensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAppCors(this IApplicationBuilder app)
    {
        app.UseCors(p =>
        {
            p.AllowAnyOrigin();
            p.WithMethods("GET");
            p.AllowAnyHeader();
        });

        return app;
    }
}

