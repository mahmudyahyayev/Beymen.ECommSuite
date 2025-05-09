namespace BuildingBlocks.Abstractions.Template.Render
{
    public interface IRenderService
    {
        Task<string> RenderAsync(string template, object view);
    }
}
