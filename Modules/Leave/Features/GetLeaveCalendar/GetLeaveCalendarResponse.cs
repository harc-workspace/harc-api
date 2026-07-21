namespace Harc.Api.Modules.Leave.Features.GetLeaveCalendar;

public class GetLeaveCalendarResponse
{
    public List<LeaveEvent> MyLeaves { get; set; } = [];
    public List<TeamLeaveEvent> TeamLeaves { get; set; } = [];
    public List<HolidayEvent> Holidays { get; set; } = [];
}

public class LeaveEvent
{
    public int Id { get; set; }
    public int Type { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

public class TeamLeaveEvent
{
    public int Id { get; set; }
    public string User { get; set; } = string.Empty;
    public int Type { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

public class HolidayEvent
{
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}