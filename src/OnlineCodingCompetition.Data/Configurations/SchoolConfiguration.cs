namespace OnlineCodingCompetition.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCodingCompetition.Data.Entities;

public class SchoolConfiguration : IEntityTypeConfiguration<School>
{
    public void Configure(EntityTypeBuilder<School> builder)
    {
        builder.HasIndex(s => new { s.Name, s.City }).IsUnique();
    }
}
