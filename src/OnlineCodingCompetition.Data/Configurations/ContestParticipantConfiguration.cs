namespace OnlineCodingCompetition.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCodingCompetition.Data.Entities;

public class ContestParticipantConfiguration : IEntityTypeConfiguration<ContestParticipant>
{
    public void Configure(EntityTypeBuilder<ContestParticipant> builder)
    {
        // Съставен ключ: един потребител се записва най-много веднъж за едно състезание.
        builder.HasKey(p => new { p.ContestId, p.UserId });

        builder.HasOne(p => p.Contest)
            .WithMany(c => c.Participants)
            .HasForeignKey(p => p.ContestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.User)
            .WithMany(u => u.Participations)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
