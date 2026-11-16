namespace OnlineCodingCompetition.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCodingCompetition.Data.Entities;

public class SubmissionTestResultConfiguration : IEntityTypeConfiguration<SubmissionTestResult>
{
    public void Configure(EntityTypeBuilder<SubmissionTestResult> builder)
    {
        builder.HasKey(r => new { r.SubmissionId, r.TestCaseId });

        builder.ToTable(t => t.HasCheckConstraint("CK_SubmissionTestResults_Points", "[PointsAwarded] >= 0"));

        builder.HasOne(r => r.Submission)
            .WithMany(s => s.TestResults)
            .HasForeignKey(r => r.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.TestCase)
            .WithMany(t => t.Results)
            .HasForeignKey(r => r.TestCaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
