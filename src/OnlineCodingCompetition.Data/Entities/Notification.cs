namespace OnlineCodingCompetition.Data.Entities;

using System.ComponentModel.DataAnnotations;
using OnlineCodingCompetition.Data.Common;

/// <summary>Известие до потребител, напр. „Решението ви е проверено“ или „Състезанието започва“.</summary>
public class Notification
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;

    [Required]
    [MaxLength(DataConstants.NotificationTitleMaxLength)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(DataConstants.NotificationMessageMaxLength)]
    public string Message { get; set; } = null!;

    /// <summary>Относителен адрес в сайта, към който води известието.</summary>
    [MaxLength(DataConstants.LinkMaxLength)]
    public string? Link { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedOn { get; set; }
}
