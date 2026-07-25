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
    }
}