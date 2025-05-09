using BuildingBlocks.Core.Web.Extenions;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace BuildingBlocks.Web.Middlewares
{
    public class PropagateCorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public PropagateCorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            _ = context.GetCorrelationId();
            await _next.Invoke(context);
        }
    }
}
