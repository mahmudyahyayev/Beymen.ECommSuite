using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Web.Middlewares
{
    public class DeChunkerMiddleware
    {
        private readonly RequestDelegate _next;

        public DeChunkerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!IsSwaggerEndpoint(context))
            {
                var originalBodyStream = context.Response.Body;
                using (var responseBody = new MemoryStream())
                {
                    try
                    {
                        context.Response.Body = responseBody;
                        long responseLength = 0;
                        context.Response.OnStarting(() =>
                        {
                            context.Response.Headers.ContentLength = responseLength;
                            return Task.CompletedTask;
                        });
                        await _next(context);
                        responseBody.Seek(0, SeekOrigin.Begin);
                        var bodyText = await new StreamReader(responseBody).ReadToEndAsync();
                        responseLength = responseBody.Length;
                        responseBody.Seek(0, SeekOrigin.Begin);
                        await responseBody.CopyToAsync(originalBodyStream);
                    }
                    finally
                    {
                        context.Response.Body = originalBodyStream;
                    }
                }
            }
            else
            {
                await _next(context);
            }
        }

        //public async Task InvokeAsync(HttpContext context)
        //{
        //    if (!IsSwaggerEndpoint(context))
        //    {
        //        var originalBodyStream = context.Response.Body;
        //        using (var responseBody = new MemoryStream())
        //        {
        //            context.Response.Body = responseBody;
        //            long length = 0;
        //            context.Response.OnStarting(() =>
        //            {
        //                context.Response.Headers.ContentLength = length;
        //                return Task.CompletedTask;
        //            });
        //            await _next(context);

        //            context.Response.Body.Seek(0, SeekOrigin.Begin);
        //            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        //            length = context.Response.Body.Length;
        //            context.Response.Body.Seek(0, SeekOrigin.Begin);
        //            await responseBody.CopyToAsync(originalBodyStream);
        //        }
        //    }
        //}

        private bool IsSwaggerEndpoint(HttpContext context)
        {
            var path = context.Request.Path;
            return path.Value != null && path.Value.Contains("swagger");
        }
    }
}

