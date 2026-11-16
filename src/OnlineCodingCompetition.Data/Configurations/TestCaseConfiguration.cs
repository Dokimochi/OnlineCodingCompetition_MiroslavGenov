namespace OnlineCodingCompetition.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCodingCompetition.Data.Entities;

public class TestCaseConfiguration : IEntityTypeConfiguration<TestCase>
{
    public void Configure(EntityTypeBuilder<TestCase> builder)
    {
        builder.ToTable(t => t.HasCheckConstraint("CK_TestCases_Points", "[Points] >= 0"));

        builder.HasIndex(t => new { t.ProblemId, t.OrderIndex }).IsUnique();

        builder.HasOne(t => t.Problem)
            .WithMany(p => p.TestCases)
            .HasForeignKey(t => t.ProblemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
