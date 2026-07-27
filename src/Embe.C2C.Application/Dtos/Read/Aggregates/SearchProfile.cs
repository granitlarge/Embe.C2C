using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Domain.Errors.Aggregates;
using Embe.C2C.Domain.Errors.ValueObjects;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Domain.ValueObjects.Engagements;

namespace Embe.C2C.Application.Dtos.Read.Aggregates;

public record SearchProfileDto
(
    Guid Id,
    Guid UserId,
    bool? Active,
    string? Name,
    string? Description,
    RelationshipType? RelationshipType,
    Engagement? Engagement,
    IReadOnlyCollection<Gender>? Genders,
    int? AgeRangeMin,
    int? AgeRangeMax,
    double? MaximumDistanceKm,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);

public class SearchProfileDtoMapper
{

    private readonly SearchProfileAuthorizationService _searchProfileAuthorizationService;

    public SearchProfileDtoMapper(SearchProfileAuthorizationService searchProfileAuthorizationService)
    {
        _searchProfileAuthorizationService = searchProfileAuthorizationService;
    }

    public async Task<ReadDto<SearchProfileDto, SearchProfilePermission>?> ToDtoAsync(SearchProfile searchProfile, CancellationToken cancellationToken)
    {
        var (permissions, variant) = await _searchProfileAuthorizationService.GetAsync(searchProfile, cancellationToken);

        if (variant == SearchProfileVariant.Empty)
            return null;

        var dto = new SearchProfileDto
        (
            searchProfile.Id,
            searchProfile.UserId,
            variant.IncludeActive ? searchProfile.Active : null,
            variant.IncludeName ? searchProfile.Name : null,
            variant.IncludeDescription ? searchProfile.Description : null,
            variant.IncludeRelationshipType ? searchProfile.RelationshipType : null,
            variant.IncludeEngagement ? searchProfile.Engagement : null,
            variant.IncludeGenders ? searchProfile.Genders.Select(g => g.Gender).ToList() : null,
            variant.IncludeAgeRange ? searchProfile.AgeRangeMin?.Value : null,
            variant.IncludeAgeRange ? searchProfile.AgeRangeMax?.Value : null,
            variant.IncludeMaximumDistance ? searchProfile.MaximumDistance?.ToKilometers().Value : null,
            variant.IncludeCreatedAt ? searchProfile.CreatedAt : null,
            variant.IncludeUpdatedAt ? searchProfile.UpdatedAt : null
        );

        return new ReadDto<SearchProfileDto, SearchProfilePermission>(dto, permissions);
    }

}