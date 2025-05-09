using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BuildingBlocks.Web
{

    public class HttpQueryAttribute : HttpMethodAttribute
    {
        private static readonly IEnumerable<string> _supportedMethods = new[] { "QUERY" };

        public HttpQueryAttribute()
            : base(_supportedMethods) { }
        public HttpQueryAttribute([StringSyntax("Route")] string template)
            : base(_supportedMethods, template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }
        }
    }
}
