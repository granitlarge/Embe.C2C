using Embe.C2C.Domain.Aggregates.Candidates;
using Embe.C2C.Infrastructure.Ef.Configurations.AbstractionConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class CandidateConfiguration : AggregateConfiguration<Candidate>
{
    public override void Configure(EntityTypeBuilder<Candidate> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => new { c.UserId, c.CandidateUserId, c.UserSearchProfileId, c.CandidateSearchProfileId }).IsUnique();

        builder.HasOne(c => c.User)
            .WithMany(u => u.CandidateUsers)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.CandidateUser)
            .WithMany(u => u.CandidateCandidates)
            .HasForeignKey(c => c.CandidateUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.UserSearchProfile)
            .WithMany()
            .HasForeignKey(c => c.UserSearchProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.CandidateSearchProfile)
            .WithMany()
            .HasForeignKey(c => c.CandidateSearchProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}