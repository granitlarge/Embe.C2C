using System.Collections.Immutable;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Dtos.Read.Entities;
using Embe.C2C.Application.Dtos.Read.ValueObjects;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
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
    ImmutableHashSet<ImageDto>? Images,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt,
    double? DistanceKmToQueryingUser,
    string? Bio,
    UserSettingsDto? Settings
);

public class UserDtoMapper(ImageDtoMapper imageDtoMapper, UserAuthorizationService userAuthorizationService)
{
    private readonly ImageDtoMapper _imageDtoMapper = imageDtoMapper;
    private readonly UserAuthorizationService _userAuthorizationService = userAuthorizationService;

    public async Task<ReadDto<UserDto, UserPermission>?> ToDtoAsync
    (
        User user,
        User? queryingUser,
        CancellationToken cancellationToken = default
    )
    {
        var userEnriched = user.Enrich(queryingUser);
        var (permissions, variant) = await _userAuthorizationService.GetAsync(user.Id, cancellationToken);

        if (variant == UserVariant.Empty)
        {
            return null;
        }

        var images = await Task.WhenAll(user.Images
            .Where(i => variant.IncludeImages)
            .Select(image => _imageDtoMapper.ToDtoAsync(image, cancellationToken)));

        var dto = new UserDto
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
            variant.IncludeBio ? user.Bio : null,
            variant.IncludeSettings ? user.Settings.ToDo() : null
        );

        return new ReadDto<UserDto, UserPermission>(dto, permissions);
    }
}