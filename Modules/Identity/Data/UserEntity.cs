using System.ComponentModel.DataAnnotations;
using Harc.Api.Common.Models;

namespace Harc.Api.Modules.Identity.Data;

public enum UserStatus
{
    Active = 1,
    Passive = 2,
    Terminated = 3,
    Contractor = 4
}

public class UserEntity : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string FullName { get; set; } = string.Empty;
    
    public int RoleId { get; set; }
    public RoleEntity Role { get; set; } = null!;
    
    public int TitleId { get; set; }
    public TitleEntity Title { get; set; } = null!;
    
    public int? TeamId { get; set; }
    public TeamEntity? Team { get; set; }
    
    public Guid? ManagerId { get; set; }
    public UserEntity? Manager { get; set; }

    public DateTime EmploymentStartDate { get; set; }
    public DateTime? EmploymentEndDate { get; set; }
    public int PriorExperienceMonths { get; set; }
    public ICollection<Leave.Data.LeaveEntity> Leaves { get; set; } = new List<Leave.Data.LeaveEntity>();

    public string? AvatarUrl { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
}

