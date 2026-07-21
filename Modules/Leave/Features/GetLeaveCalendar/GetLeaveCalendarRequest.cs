using FastEndpoints;

namespace Harc.Api.Modules.Leave.Features.GetLeaveCalendar;

public class GetLeaveCalendarRequest
{
    [QueryParam]
    public int Year { get; set; }
    [QueryParam]
    public int Month { get; set; }
}