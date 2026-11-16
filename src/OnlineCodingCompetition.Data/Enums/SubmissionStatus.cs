namespace OnlineCodingCompetition.Data.Enums;

/// <summary>Състояние на изпратено решение. След проверка е първият неуспешен вердикт или Accepted.</summary>
public enum SubmissionStatus
{
    Queued = 0,
    Running = 1,
    Accepted = 2,
    WrongAnswer = 3,
    TimeLimitExceeded = 4,
    MemoryLimitExceeded = 5,
    RuntimeError = 6,
    CompilationError = 7,
    SystemError = 8,
}
