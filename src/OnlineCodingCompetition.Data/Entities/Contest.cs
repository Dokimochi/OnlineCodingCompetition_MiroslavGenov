namespace OnlineCodingCompetition.Data.Entities;

using System.ComponentModel.DataAnnotations;
using OnlineCodingCompetition.Data.Common;
using OnlineCodingCompetition.Data.Enums;

/// <summary>
/// Състезание. Всички дати се пазят в UTC.
/// Записването е възможно между RegistrationStart и RegistrationEnd,
/// а задачите се виждат и решенията се приемат между StartTime и EndTime.
/// </summary>
public class Contest
{
    public int Id { get; set; }

    [Required]
    [MaxLength(DataConstants.TitleMaxLength)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(DataConstants.DescriptionMaxLength)]
    public string Description { get; set; } = null!;

    public DateTime RegistrationStart { get; set; }

    public DateTime RegistrationEnd { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public ScoringType ScoringType { get; set; } = ScoringType.Ioi;

    /// <summary>Най-нисък допустим клас (по избор).</summary>
    public byte? MinGrade { get; set; }

    /// <summary>Най-висок допустим клас (по избор).</summary>
    public byte? MaxGrade { get; set; }

    /// <summary>Максимален брой участници (по избор).</summary>
    public int? MaxParticipants { get; set; }

    /// <summary>Само публикуваните състезания се виждат от участниците.</summary>
    public bool IsPublished { get; set; }

    [Required]
    public string CreatorId { get; set; } = null!;

    public ApplicationUser Creator { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public ICollection<Problem> Problems { get; set; } = new HashSet<Problem>();

    public ICollection<ContestParticipant> Participants { get; set; } = new HashSet<ContestParticipant>();
}
