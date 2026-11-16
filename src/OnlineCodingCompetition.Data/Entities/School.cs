namespace OnlineCodingCompetition.Data.Entities;

using System.ComponentModel.DataAnnotations;
using OnlineCodingCompetition.Data.Common;

/// <summary>Училище, от което е участникът.</summary>
public class School
{
    public int Id { get; set; }

    [Required]
    [MaxLength(DataConstants.SchoolNameMaxLength)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(DataConstants.CityMaxLength)]
    public string City { get; set; } = null!;

    public ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>();
}
