namespace OnlineCodingCompetition.Data.Entities;

using System.ComponentModel.DataAnnotations;
using OnlineCodingCompetition.Data.Common;

/// <summary>Език, на който може да се изпрати решение, и командите за компилиране и пускане.</summary>
public class ProgrammingLanguage
{
    public int Id { get; set; }

    [Required]
    [MaxLength(DataConstants.LanguageNameMaxLength)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(DataConstants.FileExtensionMaxLength)]
    public string FileExtension { get; set; } = null!;

    /// <summary>Празно за езици, които не се компилират (Python, JavaScript).</summary>
    [MaxLength(DataConstants.CommandMaxLength)]
    public string? CompileCommand { get; set; }

    [Required]
    [MaxLength(DataConstants.CommandMaxLength)]
    public string RunCommand { get; set; } = null!;

    public bool IsEnabled { get; set; } = true;

    public ICollection<Submission> Submissions { get; set; } = new HashSet<Submission>();
}
