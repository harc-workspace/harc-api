using Harc.Api.Common.Models;

namespace Harc.Api.Modules.Document.Data;

public class DocumentEntity : BaseEntity
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public Guid UserId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string RelatedEntityId { get; set; } = string.Empty;
}