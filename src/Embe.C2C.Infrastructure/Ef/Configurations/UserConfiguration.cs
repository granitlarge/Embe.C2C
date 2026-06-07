using System.Collections.Immutable;
using Embe.C2C.Domain.Aggregates.Accounts;
using Embe.C2C.Domain.Aggregates.Judgements;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

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
            .HasMany<Domain.Entities.File>("_files")
            .WithOne()
            .HasForeignKey(f => f.OwnerUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}