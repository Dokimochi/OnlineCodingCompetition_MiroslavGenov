namespace OnlineCodingCompetition.Data.Entities;

using System.ComponentModel.DataAnnotations;
using OnlineCodingCompetition.Data.Common;
using OnlineCodingCompetition.Data.Enums;

/// <summary>Резултат от едно решение върху един тест.</summary>
public class SubmissionTestResult
{
    public int SubmissionId { get; set; }

    public Submission Submission { get; set; } = null!;

    public int TestCaseId { get; set; }

    public TestCase TestCase { get; set; } = null!;

    public TestVerdict Verdict { get; set; }

    public int ExecutionTimeMs { get; set; }

    public int MemoryUsedKb { get; set; }

    public int PointsAwarded { get; set; }

    [MaxLength(DataConstants.CheckerMessageMaxLength)]
    public string? CheckerMessage { get; set; }
}
