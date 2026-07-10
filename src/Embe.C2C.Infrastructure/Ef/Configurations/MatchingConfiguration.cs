using Embe.C2C.Infrastructure.Ef.Configurations.AbstractionConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class MatchingConfiguration : AggregateConfiguration<Domain.Aggregates.Matchings.Matching>
{
    public override void Configure(EntityTypeBuilder<Domain.Aggregates.Matchings.Matching> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasOne(m => m.User1)
            .WithMany(u => u.Matchings1)
            .HasForeignKey(m => m.UserId1)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.User2)
            .WithMany(u => u.Matchings2)
            .HasForeignKey(m => m.UserId2)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(m => m.User1SearchProfile)
            .WithMany(sp => sp.MatchingsUserId1)
            .HasForeignKey(m => m.UserId1SearchProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.User2SearchProfile)
            .WithMany(sp => sp.MatchingsUserId2)
            .HasForeignKey(m => m.UserId2SearchProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        base.Configure(builder);
    }
}