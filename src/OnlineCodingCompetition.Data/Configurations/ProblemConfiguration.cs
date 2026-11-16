namespace OnlineCodingCompetition.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCodingCompetition.Data.Entities;

public class ProblemConfiguration : IEntityTypeConfiguration<Problem>
{
    public void Configure(EntityTypeBuilder<Problem> builder)
    {
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Problems_TimeLimit", "[TimeLimitMs] > 0");
            t.HasCheckConstraint("CK_Problems_MemoryLimit", "[MemoryLimitMb] > 0");
        });

        builder.HasIndex(p => new { p.ContestId, p.OrderIndex }).IsUnique();

        builder.HasOne(p => p.Contest)
            .WithMany(c => c.Problems)
            .HasForeignKey(p => p.ContestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
