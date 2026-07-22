using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Messages;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Infrastructure.Ef.Configurations.AbstractionConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class MessageConfiguration : AggregateConfiguration<Message>
{
    public override void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);
        builder.HasOne(m => m.Matching)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.MatchingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.ReplyToMessage)
            .WithMany()
            .HasForeignKey(m => m.ReplyToMessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.AuthorUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(m => m.Content)
            .HasConversion
            (
                m => m.Value,
                v => MessageContent.Create(v)
            );

        base.Configure(builder);
    }
}