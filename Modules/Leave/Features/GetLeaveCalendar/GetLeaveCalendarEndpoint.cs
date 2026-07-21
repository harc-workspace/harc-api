using FastEndpoints;
using Harc.Api.Modules.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Harc.Api.Modules.Leave.Features.GetLeaveCalendar;

public class GetLeaveCalendarEndpoint : Endpoint<GetLeaveCalendarRequest, GetLeaveCalendarResponse>
{
    private readonly IdentityDbContext _dbContext;

    public GetLeaveCalendarEndpoint(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("api/leave/calendar");
        Description(b => b.WithName("GetLeaveCalendar").WithTags("Leave"));
    }

    public override async Task HandleAsync(GetLeaveCalendarRequest request, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst("harc_user_id");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var myLeaves = await _dbContext.Leaves
            .Where(l => l.UserId == userId && l.StartDate.Year == request.Year && l.StartDate.Month == request.Month)
            .Select(l => new LeaveEvent
            {
                Id = l.Id,
                Type = (int)l.Type,
                Status = l.Status.ToString(),
                Start = l.StartDate,
                End = l.EndDate
            })
            .ToAsyncEnumerable()
            .ToListAsync(ct);

        var userTeamId = await _dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => u.TeamId)
            .FirstOrDefaultAsync(ct);

        var teamLeaves = new List<TeamLeaveEvent>();
        if (userTeamId != null)
        {
            teamLeaves = await _dbContext.Leaves
                .Where(l => l.User.TeamId == userTeamId
                         && l.UserId != userId
                         && l.StartDate.Year == request.Year
                         && l.StartDate.Month == request.Month)
                .Select(l => new TeamLeaveEvent
                {
                    Id = l.Id,
                    User = l.User.FullName,
                    Type = (int)l.Type,
                    Start = l.StartDate,
                    End = l.EndDate
                })
                .ToListAsync(ct);
        }

        var holidays = await _dbContext.Holidays
            .Where(h => h.Date.Year == request.Year && h.Date.Month == request.Month)
            .Select(h => new HolidayEvent
            {
                Name = h.Name,
                Date = h.Date
            })
            .ToAsyncEnumerable()
            .ToListAsync(ct);

        var response = new GetLeaveCalendarResponse
        {
            MyLeaves = myLeaves,
            TeamLeaves = teamLeaves,
            Holidays = holidays
        };

        await Send.OkAsync(response, cancellation: ct);
    }
}