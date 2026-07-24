using System.Collections.Immutable;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Dtos.Read.Entities;
using Embe.C2C.Application.Dtos.Read.ValueObjects;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Application.Enrichment.Aggregates;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Dtos.Read.Aggregates;

public record UserDto
(
    Guid Id,
    string? Email,
    string? Alias,
    DateOnly? BirthDate,
    int? Age,
    Gender? Gender,
    LocationDto? Location,
    ImmutableHashSet<ImageDto>? Images,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt,
    double? DistanceKmToQueryingUser,
    string? Bio
);

public class UserDtoMapper
{
    private readonly ImageDtoMapper _imageDtoMapper;

    public UserDtoMapper(ImageDtoMapper imageDtoMapper)
    {
        _imageDtoMapper = imageDtoMapper;
    }

    public async Task<UserDto?> ToDtoAsync
    (
        UserEnriched userEnriched,
        UserVariant variant,
        CancellationToken cancellationToken = default
    )
    {
        if (variant == UserVariant.Empty)
        {
            return null;
        }

        var user = userEnriched.User;
        var images = await Task.WhenAll(user.Images
            .Where(i => variant.IncludeImages)
            .Select(image => _imageDtoMapper.ToDtoAsync(image, cancellationToken)));

        return new UserDto
        (
            user.Id,
            variant.IncludeEmail ? user.Email.Value : null,
            variant.IncludeAlias ? user.Alias.Value : null,
            variant.IncludeBirthDate ? user.BirthDate.Value : null,
            variant.IncludeAge ? user.Age.Value : null,
            variant.IncludeGender ? user.Gender : null,
            variant.IncludeLocation ? user.Location?.ToDto() : null,
            variant.IncludeImages ? [.. images] : null,
            variant.IncludeCreatedAt ? user.CreatedAt : null,
            variant.IncludeUpdatedAt ? user.UpdatedAt : null,
            variant.IncludeDistanceToQueryingUser ? userEnriched.DistanceKmToQueryingUser : null,
            variant.IncludeBio ? user.Bio : null
        );
    }
}