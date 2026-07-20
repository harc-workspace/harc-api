namespace Harc.Api.Modules.Document.Services;

public interface IDocumentManager
{
    Task UploadDocumentsAsync(List<IFormFile> files, Guid ownerUserId, string documentType, string relatedEntityId, CancellationToken ct);
}