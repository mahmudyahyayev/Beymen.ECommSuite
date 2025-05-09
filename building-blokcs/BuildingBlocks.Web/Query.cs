using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Web
{
    public class Query
    {
        public string? Text { get; set; }

        public static async ValueTask<Query> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            string? text = null;
            var request = context.Request;

            if (!request.Body.CanSeek)
            {
                request.EnableBuffering();
            }

            if (request.Body.CanRead)
            {
                request.Body.Position = 0;
                var reader = new StreamReader(request.Body, Encoding.UTF8);
                text = await reader.ReadToEndAsync().ConfigureAwait(false);
                request.Body.Position = 0;
            }

            return new Query { Text = text };
        }

        public static implicit operator string(Query query)
        {
            return query.Text ?? string.Empty; 
        }
    }
}
