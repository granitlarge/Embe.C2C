using System.Collections.Immutable;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Dtos.Read.Entities;
using Embe.C2C.Application.Dtos.Read.ValueObjects;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Users;
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
    ImageDto? ProfilePicture,
    ImmutableHashSet<ImageDto>? Images,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
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
        User user,
        UserVariant variant,
        CancellationToken cancellationToken = default
    )
    {
        if (variant == UserVariant.Empty)
        {
            return null;
        }

        var images = user.Images != null && variant.IncludeImages ? await Task.WhenAll(user.Images.Select(f => _imageDtoMapper.ToDtoAsync(f, cancellationToken))) : null;
        var profilePicture = user.ProfilePicture != null && variant.IncludeProfilePicture ? await _imageDtoMapper.ToDtoAsync(user.ProfilePicture, cancellationToken) : null;

        return new UserDto
        (
            user.Id,
            variant.IncludeEmail ? user.Email.Value : null,
            variant.IncludeAlias ? user.Alias.Value : null,
            variant.IncludeBirthDate ? user.BirthDate.Value : null,
            variant.IncludeAge ? user.Age.Value : null,
            variant.IncludeGender ? user.Gender : null,
            variant.IncludeLocation ? user.Location?.ToDto() : null,
            variant.IncludeProfilePicture ? profilePicture : null,
            variant.IncludeImages ? images?.ToImmutableHashSet() : null,
            variant.IncludeCreatedAt ? user.CreatedAt : null,
            variant.IncludeUpdatedAt ? user.UpdatedAt : null
        );
    }
}