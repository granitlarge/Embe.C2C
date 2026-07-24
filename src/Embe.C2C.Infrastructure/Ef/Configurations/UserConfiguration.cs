using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Infrastructure.Ef.Configurations.AbstractionConfigurations;
using Embe.C2C.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class UserConfiguration : AggregateConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Gender);

        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value))
            .IsRequired();

        builder.Property(u => u.Alias)
            .HasConversion(
                userName => userName.Value,
                value => Alias.Create(value))
            .IsRequired();

        builder.Property(u => u.BirthDate)
            .HasConversion(
                birthDate => birthDate.Value,
                value => new BirthDate(value))
            .IsRequired();

        builder.Property(u => u.Coordinates).HasColumnType("geography");
        builder.HasIndex(x => x.Coordinates).HasMethod("GIST");

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.IdentityUserId).IsUnique();

        builder.HasOne<MyIdentityUser>()
            .WithOne()
            .HasForeignKey<User>(u => u.IdentityUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .OwnsMany<Domain.Entities.Image>("_images", image =>
            {
                image.HasKey(image => image.Id);
                image.Property(image => image.Id).ValueGeneratedNever();
                image.WithOwner().HasForeignKey(image => image.OwnerUserId);
                image.OwnsOne(image => image.ImageDetails, fd =>
                {
                    fd.Property(d => d.Name);
                    fd.Property(d => d.MimeType);
                    fd.Property(d => d.Order);
                    fd.HasIndex(d => d.Name).IsUnique();
                });
                image.Property(image => image.RowVersion).IsRowVersion();
            });

        base.Configure(builder);
    }
}