using Harc.Api.Modules.Document.Data;
using Harc.Api.Modules.Identity.Data;

namespace Harc.Api.Modules.Document.Services;

public class DocumentManager : IDocumentManager
{
    private readonly IWebHostEnvironment _env;
    private readonly IdentityDbContext _dbContext;

    public DocumentManager(IWebHostEnvironment env, IdentityDbContext dbContext)
    {
        _env = env;
        _dbContext = dbContext;
    }

    public async Task UploadDocumentsAsync(List<IFormFile> files, Guid ownerUserId, string documentType, string relatedEntityId, CancellationToken ct)
    {
        if (files == null || !files.Any()) return;

        var uploadFolder = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, "wwwroot", "uploads", "documents");
        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var fileExtension = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var physicalFilePath = Path.Combine(uploadFolder, uniqueFileName);

                // Dosyayı diske kaydet
                using (var stream = new FileStream(physicalFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream, ct);
                }

                // Veritabanı kaydını oluştur
                var newDocument = new DocumentEntity
                {
                    UserId = ownerUserId,
                    FileName = file.FileName,
                    FilePath = $"/uploads/documents/{uniqueFileName}",
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    DocumentType = documentType,
                    RelatedEntityId = relatedEntityId
                };

                _dbContext.Documents.Add(newDocument);
            }
        }
        
        // Değişiklikleri veritabanına yansıt
        await _dbContext.SaveChangesAsync(ct);
    }
}