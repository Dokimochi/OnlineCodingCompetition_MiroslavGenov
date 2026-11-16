namespace OnlineCodingCompetition.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCodingCompetition.Data.Entities;

public class ContestConfiguration : IEntityTypeConfiguration<Contest>
{
    public void Configure(EntityTypeBuilder<Contest> builder)
    {
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Contests_RegistrationPeriod", "[RegistrationStart] < [RegistrationEnd]");
            t.HasCheckConstraint("CK_Contests_RegistrationBeforeStart", "[RegistrationEnd] <= [StartTime]");
            t.HasCheckConstraint("CK_Contests_Duration", "[StartTime] < [EndTime]");
            t.HasCheckConstraint("CK_Contests_Grades", "[MinGrade] IS NULL OR [MaxGrade] IS NULL OR [MinGrade] <= [MaxGrade]");
            t.HasCheckConstraint("CK_Contests_MaxParticipants", "[MaxParticipants] IS NULL OR [MaxParticipants] > 0");
        });

        builder.HasIndex(c => c.StartTime);

        // Състезание не се изтрива заедно с организатора си.
        builder.HasOne(c => c.Creator)
            .WithMany(u => u.CreatedContests)
            .HasForeignKey(c => c.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
