namespace OnlineCodingCompetition.Data;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineCodingCompetition.Data.Entities;

/// <summary>
/// Контекст на базата данни (EF Core Code First).
/// Таблиците за потребители и роли идват от ASP.NET Core Identity,
/// а връзките и ограниченията са описани в папка Configurations.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<School> Schools => this.Set<School>();

    public DbSet<Contest> Contests => this.Set<Contest>();

    public DbSet<ContestParticipant> ContestParticipants => this.Set<ContestParticipant>();

    public DbSet<Problem> Problems => this.Set<Problem>();

    public DbSet<TestCase> TestCases => this.Set<TestCase>();

    public DbSet<ProgrammingLanguage> ProgrammingLanguages => this.Set<ProgrammingLanguage>();

    public DbSet<Submission> Submissions => this.Set<Submission>();

    public DbSet<SubmissionTestResult> SubmissionTestResults => this.Set<SubmissionTestResult>();

    public DbSet<Notification> Notifications => this.Set<Notification>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
