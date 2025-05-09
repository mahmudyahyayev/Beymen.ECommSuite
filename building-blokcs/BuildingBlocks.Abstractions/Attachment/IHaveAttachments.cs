using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Abstractions.Attachment
{
    public interface IHaveAttachments
    {
        IEnumerable<IFormFile> Attachments { get; set; }
    }
}
