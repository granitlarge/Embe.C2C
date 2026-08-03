using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class MyIdentityUserConfiguration : IEntityTypeConfiguration<MyIdentityUser>
{
    public void Configure(EntityTypeBuilder<MyIdentityUser> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Email).IsUnique();
        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<MyIdentityUser>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(e => e.UserId).IsUnique();
    }
}