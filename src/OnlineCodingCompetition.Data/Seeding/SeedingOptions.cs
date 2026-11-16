namespace OnlineCodingCompetition.Data.Seeding;

/// <summary>
/// Настройки за началните данни (секция "Seeding" в конфигурацията).
/// Паролата на демо акаунтите не е в хранилището: задава се с User Secrets
/// при разработка и с променлива на средата на сървъра.
/// </summary>
public class SeedingOptions
{
    public const string SectionName = "Seeding";

    /// <summary>Дали при стартиране да се добавят демо данни.</summary>
    public bool Enabled { get; set; }

    /// <summary>Обща парола за демо акаунтите.</summary>
    public string? DemoPassword { get; set; }
}
