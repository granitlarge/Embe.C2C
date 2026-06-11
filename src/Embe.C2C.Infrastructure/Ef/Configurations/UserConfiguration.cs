using System.Collections.Immutable;
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
            gender => Enum.GetName(gender)!,
            value => Enum.Parse<Gender>(value)
        );

        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value))
            .IsRequired();

        builder.Property(u => u.UserName)
            .HasConversion(
                userName => userName.Value,
                value => UserName.Create(value))
            .IsRequired();

        builder.Property(u => u.BirthDate)
            .HasConversion(
                birthDate => birthDate.Value,
                value => new BirthDate(value))
            .IsRequired();

        builder
            .ComplexProperty(u => u.DatingPreferences, dp =>
            {
                dp.Property(d => d.AgeRangeMin)
                    .HasConversion(
                        age => age.Value,
                        value => new Age(value))
                    .IsRequired();

                dp.Property(d => d.AgeRangeMax)
                    .HasConversion(
                        age => age.Value,
                        value => new Age(value))
                    .IsRequired();

                dp.Property(d => d.MaximumDistance)
                    .HasConversion(
                        distance => distance.ToKilometers().Value,
                        value => new Distance(value, LengthUnit.Kilometers))
                    .IsRequired();

                dp.Property(d => d.InterestedInGenders)
                    .HasConversion(
                        genders => string.Join(',', genders.Select(g => Enum.GetName(g))),
                        value => value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(e => Enum.Parse<Gender>(e)).ToImmutableHashSet())
                    .IsRequired();
            });

        builder.Property(u => u.Location)
            .HasConversion(
                location => location == null ? null : new Point(location.Longitude, location.Latitude),
                value => value == null ? null : new Domain.ValueObjects.Location(value.Y, value.X)
            );

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.IdentityUserId).IsUnique();

        builder.HasOne<MyIdentityUser>()
            .WithOne()
            .HasForeignKey<User>(u => u.IdentityUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .OwnsMany<Domain.Entities.File>("_files", f =>
            {
                f.HasKey(f => f.Id);
                f.WithOwner().HasForeignKey(f => f.OwnerUserId);
                f.OwnsOne(f => f.FileDetails, fd =>
                {
                    fd.Property(d => d.Name);
                    fd.Property(d => d.MimeType);
                    fd.Property(d => d.Order);
                });
                f.Property(f => f.RowVersion).IsRowVersion();
            });

        base.Configure(builder);
    }
}