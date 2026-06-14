using Embe.C2C.Infrastructure.Ef.Configurations.AbstractionConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class BlockingConfiguration : AggregateConfiguration<Domain.Aggregates.Blockings.Blocking>
{
    public override void Configure(EntityTypeBuilder<Domain.Aggregates.Blockings.Blocking> builder)
    {
        builder.HasKey(b => b.Id);

        builder.HasOne<Domain.Aggregates.Users.User>()
            .WithMany(u => u.Blocked)
            .HasForeignKey(b => b.BlockerUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Domain.Aggregates.Users.User>()
            .WithMany(u => u.BlockedBy)
            .HasForeignKey(b => b.BlockedUserId)
            .OnDelete(DeleteBehavior.ClientCascade);

        base.Configure(builder);
    }
}