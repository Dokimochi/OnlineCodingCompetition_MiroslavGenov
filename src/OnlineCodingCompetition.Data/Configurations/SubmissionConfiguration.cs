namespace OnlineCodingCompetition.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCodingCompetition.Data.Entities;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable(t => t.HasCheckConstraint("CK_Submissions_Points", "[Points] IS NULL OR [Points] >= 0"));

        // Опашката за проверка: най-старите решения със статус Queued.
        builder.HasIndex(s => new { s.Status, s.SubmittedOn });
        builder.HasIndex(s => new { s.UserId, s.ProblemId });

        // Решенията са история на състезанието: задача, потребител или език
        // с изпратени решения не може да се изтрие.
        builder.HasOne(s => s.Problem)
            .WithMany(p => p.Submissions)
            .HasForeignKey(s => s.ProblemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.User)
            .WithMany(u => u.Submissions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.ProgrammingLanguage)
            .WithMany(l => l.Submissions)
            .HasForeignKey(s => s.ProgrammingLanguageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
