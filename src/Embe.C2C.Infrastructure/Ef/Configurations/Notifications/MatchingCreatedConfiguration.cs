using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Infrastructure.Ef.Configurations.Notifications;

public class MatchingCreatedConfiguration : IEntityTypeConfiguration<MatchingCreated>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<MatchingCreated> builder)
    {
        builder.HasBaseType<Notification>();
        builder.HasOne<Matching>()
            .WithMany()
            .HasForeignKey(n => n.MatchingId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}