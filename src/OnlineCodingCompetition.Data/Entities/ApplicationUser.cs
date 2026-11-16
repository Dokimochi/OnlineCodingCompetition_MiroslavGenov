namespace OnlineCodingCompetition.Data.Entities;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using OnlineCodingCompetition.Data.Common;

/// <summary>Потребител на платформата: участник, организатор или администратор.</summary>
public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(DataConstants.PersonNameMaxLength)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(DataConstants.PersonNameMaxLength)]
    public string LastName { get; set; } = null!;

    /// <summary>Текущ клас (1–12). Попълва се само за ученици.</summary>
    [Range(DataConstants.MinGrade, DataConstants.MaxGrade)]
    public byte? Grade { get; set; }

    public int? SchoolId { get; set; }

    public School? School { get; set; }

    public DateTime CreatedOn { get; set; }

    public ICollection<Contest> CreatedContests { get; set; } = new HashSet<Contest>();

    public ICollection<ContestParticipant> Participations { get; set; } = new HashSet<ContestParticipant>();

    public ICollection<Submission> Submissions { get; set; } = new HashSet<Submission>();

    public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
}
