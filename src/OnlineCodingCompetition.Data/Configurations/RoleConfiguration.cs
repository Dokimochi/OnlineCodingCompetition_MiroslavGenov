namespace OnlineCodingCompetition.Data.Configurations;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCodingCompetition.Data.Common;

/// <summary>Трите роли се създават от миграцията (HasData), с постоянни Id.</summary>
public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            CreateRole("a1f0c0de-0000-4000-8000-000000000001", RoleNames.Administrator),
            CreateRole("a1f0c0de-0000-4000-8000-000000000002", RoleNames.Organizer),
            CreateRole("a1f0c0de-0000-4000-8000-000000000003", RoleNames.Contestant));
    }

    private static IdentityRole CreateRole(string id, string name) => new()
    {
        Id = id,
        Name = name,
        NormalizedName = name.ToUpperInvariant(),
        ConcurrencyStamp = id,
    };
}
