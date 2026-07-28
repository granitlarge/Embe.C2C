using Embe.C2C.Domain.Aggregates.Candidates;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.Notifications.Candidates;
using Embe.C2C.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations.Notifications.Candidates;

public class PositivelyJudgedConfiguration : IEntityTypeConfiguration<PositivelyJudgedNotification>
{
    public void Configure(EntityTypeBuilder<PositivelyJudgedNotification> builder)
    {
        builder.HasBaseType<Notification>();
        builder.HasOne<Candidate>()
            .WithMany()
            .HasForeignKey(n => n.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(n => n.CandidateUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}