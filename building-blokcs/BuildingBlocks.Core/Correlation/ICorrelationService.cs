using BuildingBlocks.Abstractions.Correlation;
using BuildingBlocks.Core.Web.Extenions;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Correlation
{
    public class CorrelationService(
        IHttpContextAccessor httpContextAccessor) : ICorrelationService
    {
        public string CorrelationId
        {
            get
            {
                var correlationId = string.Empty;

                if (httpContextAccessor.HttpContext != null)
                    correlationId = httpContextAccessor.HttpContext.GetCorrelationId();
                
                return correlationId;
            }
        }
    }
}
