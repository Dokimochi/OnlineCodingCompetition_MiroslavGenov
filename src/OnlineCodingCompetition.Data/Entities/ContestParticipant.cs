namespace OnlineCodingCompetition.Data.Entities;

/// <summary>
/// Записване на потребител за състезание (връзка много-към-много между Contest и ApplicationUser).
/// Класът се пази към момента на записване, защото в профила се сменя всяка година.
/// </summary>
public class ContestParticipant
{
    public int ContestId { get; set; }

    public Contest Contest { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;

    public DateTime RegisteredOn { get; set; }

    public byte? Grade { get; set; }

    /// <summary>Дисквалифицираните участници не влизат в класирането.</summary>
    public bool IsDisqualified { get; set; }
}
