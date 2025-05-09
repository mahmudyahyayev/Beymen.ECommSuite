using System.Diagnostics;
using BuildingBlocks.Abstractions.CQRS.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BuildingBlocks.Core.Web.Extenions
{
    public static class HttpContextExtensions
    {
        private static string CorrelationIdKey = "CorrelationId";

        public static string? GetTraceId(this IHttpContextAccessor httpContextAccessor)
        {
            return Activity.Current?.TraceId.ToString() ?? httpContextAccessor?.HttpContext?.TraceIdentifier;
        }

        public static string GetCorrelationId(this HttpContext httpContext)
        {
            httpContext.Request.Headers.TryGetValue(CorrelationIdKey, out var correlationId);
            if (correlationId.Count == 0)
            {
                httpContext.Response.Headers.TryGetValue(CorrelationIdKey, out correlationId);
            }

            var correlation = correlationId.FirstOrDefault();

            if (string.IsNullOrEmpty(correlation))
            {
                correlationId = Guid.NewGuid().ToString();

                httpContext.Response.Headers.Append(CorrelationIdKey, correlationId);
                httpContext.Request.Headers.Append(CorrelationIdKey, correlationId);
            }

            return correlationId;
        }


        public static TResult? ExtractXQueryObjectFromHeader<TResult>(this HttpContext httpContext, string query)
            where TResult : ILoadOptionsRequest, new()
        {
            var queryModel = new TResult();
            if (!(string.IsNullOrEmpty(query) || query == "{}"))
            {
                queryModel = JsonConvert.DeserializeObject<TResult>(query);
            }

            httpContext?.Response.Headers.Append(
                "x-query",
                JsonConvert.SerializeObject(
                    queryModel,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }
                )
            );

            return queryModel;
        }

        public static string? GetUserHostAddress(this HttpContext context)
        {
            return context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                ?? context.Connection.RemoteIpAddress?.ToString();
        }
    }
}
