using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Infrastructure.Ef.Configurations.AbstractionConfigurations;
using Embe.C2C.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class UserConfiguration : AggregateConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Gender).HasConversion
        (
            gender => gender != null ? Enum.GetName(gender.Value)! : null,
            value => value != null ? Enum.Parse<Gender>(value) : null
        );

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

        builder.Property(u => u.Location)
            .HasConversion(
                location => location == null ? null : new Point(location.Longitude, location.Latitude)
                {
                    SRID = 4326
                },
                value => value == null ? null : new Domain.ValueObjects.Location(value.Y, value.X)
            )
            .HasColumnType("geography");

        builder.HasIndex(x => x.Location).HasMethod("GIST");

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.IdentityUserId).IsUnique();

        builder.HasOne<MyIdentityUser>()
            .WithOne()
            .HasForeignKey<User>(u => u.IdentityUserId)
            .OnDelete(DeleteBehavior.NoAction);

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
                });
                image.Property(image => image.RowVersion).IsRowVersion();
            });
    

        base.Configure(builder);
    }
}