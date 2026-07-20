using System.ComponentModel.DataAnnotations;
using Harc.Api.Common.Models;

namespace Harc.Api.Modules.Identity.Data;

public class RoleEntity : BaseEntity
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public Dictionary<string, string> DisplayName { get; set; } = new();

    public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
}