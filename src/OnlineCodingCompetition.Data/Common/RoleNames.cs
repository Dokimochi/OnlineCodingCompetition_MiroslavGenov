namespace OnlineCodingCompetition.Data.Common;

/// <summary>Ролите в системата.</summary>
public static class RoleNames
{
    /// <summary>Управлява потребителите и всички състезания.</summary>
    public const string Administrator = "Administrator";

    /// <summary>Създава състезания, задачи и тестове и следи проверките.</summary>
    public const string Organizer = "Organizer";

    /// <summary>Записва се за състезания и изпраща решения.</summary>
    public const string Contestant = "Contestant";
}
