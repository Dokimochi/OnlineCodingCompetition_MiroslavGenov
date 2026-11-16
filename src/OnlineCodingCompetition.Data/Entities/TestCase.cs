namespace OnlineCodingCompetition.Data.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>Тест към задача: вход, очакван изход и точки.</summary>
public class TestCase
{
    public int Id { get; set; }

    public int ProblemId { get; set; }

    public Problem Problem { get; set; } = null!;

    public int OrderIndex { get; set; }

    [Required]
    public string Input { get; set; } = null!;

    [Required]
    public string ExpectedOutput { get; set; } = null!;

    public int Points { get; set; }

    /// <summary>Примерните тестове се показват в условието и обикновено носят 0 точки.</summary>
    public bool IsSample { get; set; }

    public ICollection<SubmissionTestResult> Results { get; set; } = new HashSet<SubmissionTestResult>();
}
