using BuildingBlocks.Abstractions.Template.Render;
using Stubble.Core.Builders;
using Stubble.Core.Settings;
using System.Globalization;

namespace BuildingBlocks.Core.Template.Render
{
    public class RenderService : IRenderService
    {
        private readonly StubbleBuilder _builder;
        private readonly RenderSettings _renderSettings;

        public RenderService()
        {
            _builder = new StubbleBuilder();
            _renderSettings = new RenderSettings
            {
                CultureInfo = CultureInfo.GetCultureInfo("tr-TR"),
                SkipHtmlEncoding = true
            };
        }

        public async Task<string> RenderAsync(string template, object view)
        {
            return await _builder.Build().RenderAsync(template, view, _renderSettings);
        }
    }
}
