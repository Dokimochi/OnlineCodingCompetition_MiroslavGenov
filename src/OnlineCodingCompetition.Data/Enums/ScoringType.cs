namespace OnlineCodingCompetition.Data.Enums;

/// <summary>Начин на класиране в състезание.</summary>
public enum ScoringType
{
    /// <summary>Сбор от най-добрите точки по всяка задача (частични точки, както на IOI).</summary>
    Ioi = 1,

    /// <summary>Брой напълно решени задачи, после наказателно време (както на ICPC).</summary>
    Icpc = 2,
}
