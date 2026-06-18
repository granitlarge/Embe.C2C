using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Infrastructure.Ef.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class CandidateEntityConfiguration : IEntityTypeConfiguration<CandidateEntity>
{
    public void Configure(EntityTypeBuilder<CandidateEntity> builder)
    {
        builder.HasKey(c => new { c.UserId, c.CandidateUserId });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Candidate)
            .WithMany()
            .HasForeignKey(c => c.CandidateUserId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}