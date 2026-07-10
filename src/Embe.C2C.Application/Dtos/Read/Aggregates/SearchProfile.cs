using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Domain.ValueObjects.Engagements;

namespace Embe.C2C.Application.Dtos.Read.Aggregates;

public record SearchProfileDto
(
    Guid Id,
    Guid UserId,
    string? Name,
    string? Description,
    RelationshipType? RelationshipType,
    Engagement? Engagement,
    IReadOnlyCollection<Gender>? Genders,
    int? AgeRangeMin,
    int? AgeRangeMax,
    double? MaximumDistanceKm
);

public class SearchProfileDtoMapper
{
    public SearchProfileDtoMapper()
    {

    }

    public SearchProfileDto? ToDto(SearchProfile searchProfile, SearchProfileVariant variant)
    {
        if (variant == SearchProfileVariant.Empty)
            return null;

        return new SearchProfileDto
        (
            searchProfile.Id,
            searchProfile.UserId,
            variant.IncludeName ? searchProfile.Name : null,
            variant.IncludeDescription ? searchProfile.Description : null,
            variant.IncludeRelationshipType ? searchProfile.RelationshipType : null,
            variant.IncludeEngagement ? searchProfile.Engagement : null,
            variant.IncludeGenders ? searchProfile.Genders.Select(g => g.Gender).ToList() : null,
            variant.IncludeAgeRange ? searchProfile.AgeRangeMin?.Value : null,
            variant.IncludeAgeRange ? searchProfile.AgeRangeMax?.Value : null,
            variant.IncludeMaximumDistance ? searchProfile.MaximumDistance?.ToKilometers().Value : null
        );
    }
}