namespace OnlineCodingCompetition.Data.Entities;

using System.ComponentModel.DataAnnotations;
using OnlineCodingCompetition.Data.Common;
using OnlineCodingCompetition.Data.Enums;

/// <summary>Задача в състезание. Максималните точки са сборът от точките на тестовете ѝ.</summary>
public class Problem
{
    public int Id { get; set; }

    public int ContestId { get; set; }

    public Contest Contest { get; set; } = null!;

    [Required]
    [MaxLength(DataConstants.TitleMaxLength)]
    public string Title { get; set; } = null!;

    /// <summary>Условие на задачата (Markdown).</summary>
    [Required]
    public string Statement { get; set; } = null!;

    /// <summary>Пореден номер в състезанието (A, B, C …).</summary>
    public int OrderIndex { get; set; }

    /// <summary>Лимит за време за един тест, в милисекунди.</summary>
    public int TimeLimitMs { get; set; }

    /// <summary>Лимит за памет, в мегабайти.</summary>
    public int MemoryLimitMb { get; set; }

    public CheckerType CheckerType { get; set; } = CheckerType.Tokens;

    public DateTime CreatedOn { get; set; }

    public ICollection<TestCase> TestCases { get; set; } = new HashSet<TestCase>();

    public ICollection<Submission> Submissions { get; set; } = new HashSet<Submission>();
}
