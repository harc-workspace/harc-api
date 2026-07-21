using Harc.Api.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Harc.Api.Common.Data;

public class HolidayEntity : BaseEntity
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }
}