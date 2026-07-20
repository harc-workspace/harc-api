using FastEndpoints;
using System.Security.Claims;
using Harc.Api.Modules.Leave.Data;
using Harc.Api.Modules.Identity.Data;
using Harc.Api.Modules.Document.Services;

namespace Harc.Api.Modules.Leave.Features.CreateLeave;

public class CreateLeave : Endpoint<CreateLeaveRequest, CreateLeaveResponse>
{
    private readonly IdentityDbContext _dbContext;
    private readonly IDocumentManager _documentManager;

    public CreateLeave(IdentityDbContext dbContext, IDocumentManager documentManager)
    {
        _dbContext = dbContext;
        _documentManager = documentManager;
    }

    public override void Configure()
    {
        Post("api/leave");
        Description(b => b.WithName("CreateLeave").WithTags("Leave"));
    }

    public override async Task<CreateLeaveResponse> HandleAsync(CreateLeaveRequest request, CancellationToken ct)
    {
        // Validations
        // Check if the user is exists and authenticated
        // Check for overlapping leaves
        // Validate leave start and end dates (start date should be before end date, and both should be in the future)
        // Calculate total work days in the requested leave period and validate balance (Monday to Friday, excluding weekends and public holidays)
        // Validate leave type
        // Validate leave duration (should not exceed the maximum allowed for the leave type)
        
        // 1. Kullanıcı ID'sini Token'dan (Claims) al (Güvenlik için frontend'e güvenmiyoruz)
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            await Send.UnauthorizedAsync(ct); // 401 Döner
            return new CreateLeaveResponse
            {
                Success = false,
                Message = "Geçersiz kullanıcı."
            };
        }

        // 2. Yeni İzin (Leave) kaydını oluştur
        var newLeave = new Data.LeaveEntity
        {
            UserId = userId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Days = (request.EndDate - request.StartDate).TotalDays + 1, // +1 to include the start date
            Type = request.LeaveType,
            Status = LeaveStatus.Pending,
            Description = request.Description
        };

        // 3. Veritabanına kaydet
        _dbContext.Leaves.Add(newLeave);
        await _dbContext.SaveChangesAsync(ct);

        // 4. Belgeler varsa IDocumentManager'a devret
        if (request.Documents != null && request.Documents.Any())
        {
            await _documentManager.UploadDocumentsAsync(
                files: request.Documents,
                ownerUserId: userId,
                documentType: "Leave",
                relatedEntityId: newLeave.Id.ToString(),
                ct: ct
            );
        }

        // 5. Başarılı yanıt dön
        var response = new CreateLeaveResponse 
        { 
            Success = true, 
            Message = "İzin talebi ve belgeler başarıyla oluşturuldu." 
        };

        await Send.OkAsync(response, cancellation: ct);
        return response;
    }
}