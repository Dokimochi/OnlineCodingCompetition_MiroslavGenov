namespace OnlineCodingCompetition.Data.Entities;

using System.ComponentModel.DataAnnotations;
using OnlineCodingCompetition.Data.Common;
using OnlineCodingCompetition.Data.Enums;

/// <summary>Изпратено решение на задача.</summary>
public class Submission
{
    public int Id { get; set; }

    public int ProblemId { get; set; }

    public Problem Problem { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;

    public int ProgrammingLanguageId { get; set; }

    public ProgrammingLanguage ProgrammingLanguage { get; set; } = null!;

    [Required]
    [MaxLength(DataConstants.SourceCodeMaxLength)]
    public string SourceCode { get; set; } = null!;

    public SubmissionStatus Status { get; set; } = SubmissionStatus.Queued;

    /// <summary>Сбор от точките на преминатите тестове. Празно, докато решението не е проверено.</summary>
    public int? Points { get; set; }

    /// <summary>Съобщенията на компилатора при грешка при компилиране.</summary>
    public string? CompilerOutput { get; set; }

    /// <summary>Най-голямото време за изпълнение сред тестовете, в милисекунди.</summary>
    public int? ExecutionTimeMs { get; set; }

    /// <summary>Най-голямата използвана памет сред тестовете, в килобайти.</summary>
    public int? MemoryUsedKb { get; set; }

    public DateTime SubmittedOn { get; set; }

    public DateTime? JudgedOn { get; set; }

    public ICollection<SubmissionTestResult> TestResults { get; set; } = new HashSet<SubmissionTestResult>();
}
