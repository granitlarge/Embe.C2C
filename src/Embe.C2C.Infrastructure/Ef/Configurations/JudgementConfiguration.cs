using Embe.C2C.Domain.Aggregates.Judgements;
using Embe.C2C.Infrastructure.Ef.Configurations.AbstractionConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class JudgementConfiguration : AggregateConfiguration<Judgement>
{
    public override void Configure(EntityTypeBuilder<Judgement> builder)
    {
        builder.HasKey(j => j.Id);

        builder.HasOne(u => u.Judge)
            .WithMany(u => u.JudgementsPassed)
            .HasForeignKey(j => j.JudgeUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Domain.Aggregates.Users.User>()
            .WithMany(u => u.JudgementsReceived)
            .HasForeignKey(j => j.JudgeeUserId)
            .OnDelete(DeleteBehavior.Cascade);

        base.Configure(builder);
    }
}