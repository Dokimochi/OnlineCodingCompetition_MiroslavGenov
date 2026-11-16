namespace OnlineCodingCompetition.Data.Enums;

/// <summary>Резултат от изпълнението на решението върху един тест.</summary>
public enum TestVerdict
{
    Accepted = 1,
    WrongAnswer = 2,
    TimeLimitExceeded = 3,
    MemoryLimitExceeded = 4,
    RuntimeError = 5,
}
