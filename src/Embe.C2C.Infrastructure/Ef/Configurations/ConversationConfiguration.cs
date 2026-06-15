using Embe.C2C.Domain.Entities;
using Embe.C2C.Infrastructure.Ef.Configurations.AbstractionConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class ConversationConfiguration : EntityConfiguration<Conversation>
{
    public override void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasOne(c => c.LastMessage)
            .WithOne()
            .HasForeignKey<Conversation>(c => c.LastMessageId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(c => c.Matching)
            .WithOne(m => m.Conversation)
            .HasForeignKey<Conversation>(c => c.MatchingId)
            .OnDelete(DeleteBehavior.Cascade);

        base.Configure(builder);
    }
}