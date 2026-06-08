using Embe.C2C.Infrastructure.Ef.Configurations.AbstractionConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class MatchingConfiguration : AggregateConfiguration<Domain.Aggregates.Matchings.Matching>
{
    public override void Configure(EntityTypeBuilder<Domain.Aggregates.Matchings.Matching> builder)
    {
        builder.HasKey(m => m.Id);
        builder.HasOne<Domain.Aggregates.Users.User>()
            .WithMany()
            .HasForeignKey(m => m.UserId1)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Domain.Aggregates.Users.User>()
            .WithMany()
            .HasForeignKey(m => m.UserId2)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne<Domain.Entities.Conversation>()
            .WithOne()
            .HasForeignKey<Domain.Entities.Conversation>(c => c.MatchingId)
            .OnDelete(DeleteBehavior.Cascade);
        base.Configure(builder);
    }
}