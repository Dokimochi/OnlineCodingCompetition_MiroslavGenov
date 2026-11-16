namespace OnlineCodingCompetition.Data.Enums;

/// <summary>Как се сравнява изходът на решението с очаквания изход.</summary>
public enum CheckerType
{
    /// <summary>Точно съвпадение (без крайните празни редове).</summary>
    Exact = 1,

    /// <summary>Сравняване дума по дума, без значение колко интервала и нови редове има.</summary>
    Tokens = 2,
}
