using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Domain.Entities.SearchProfiles;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Infrastructure.Ef.Configurations.AbstractionConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class SearchProfileConfiguration : AggregateConfiguration<SearchProfile>
{
    public override void Configure(EntityTypeBuilder<SearchProfile> builder)
    {
        builder.HasKey(sp => sp.Id);
        builder.HasOne(sp => sp.User)
            .WithMany(u => u.SearchProfiles)
            .HasForeignKey(sp => sp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsMany<SearchProfileGender>("_genders", g =>
        {
            g.HasKey(g => g.Id);
            g.Property(g => g.Gender);
            g.WithOwner().HasForeignKey(g => g.SearchProfileId);
        });

        builder.ComplexProperty(sp => sp.Engagement, e =>
        {

        });

        builder.Property(sp => sp.RelationshipType);

        builder.Property(sp => sp.AgeRangeMin).HasConversion
        (
            age => age != null ? age.Value : (int?)null,
            value => value == null ? null : new Age(value.Value)
        );

        builder.Property(sp => sp.AgeRangeMax).HasConversion
        (
            age => age != null ? age.Value : (int?)null,
            value => value == null ? null : new Age(value.Value)
        );

        builder.Property(sp => sp.MaximumDistance).HasConversion
        (
            distance => distance != null ? distance.ToKilometers().Value : (double?)null,
            value => value == null ? null : new Distance(value.Value, LengthUnit.Kilometers)
        );

        base.Configure(builder);
    }
}