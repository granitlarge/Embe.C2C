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

        builder.HasOne(u => u.Candidate)
            .WithOne(c => c.Judgement)
            .HasForeignKey<Judgement>(j => j.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        base.Configure(builder);
    }
}