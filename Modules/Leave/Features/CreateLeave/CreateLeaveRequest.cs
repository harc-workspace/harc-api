using Harc.Api.Modules.Leave.Data;

namespace Harc.Api.Modules.Leave.Features.CreateLeave;

public class CreateLeaveRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public LeaveType LeaveType { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<IFormFile> Documents { get; set; } = [];
}