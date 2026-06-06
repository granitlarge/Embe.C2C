using Embe.C2C.Domain.Aggregates.Judgements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class JudgementConfiguration : IEntityTypeConfiguration<Judgement>
{
    public void Configure(EntityTypeBuilder<Judgement> builder)
    {
        builder.HasKey(j => j.Id);

        builder.HasOne<Domain.Aggregates.Users.User>()
            .WithMany()
            .HasForeignKey(j => j.JudgeUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Domain.Aggregates.Users.User>()
            .WithMany()
            .HasForeignKey(j => j.JudgeeUserId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}