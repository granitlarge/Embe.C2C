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
    string? UserName,
    DateOnly? BirthDate,
    Gender? Gender,
    DatingPreferencesDto? DatingPreferences,
    LocationDto? Location,
    FileDto? ProfilePicture,
    ImmutableHashSet<FileDto>? Files,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);

public static class UserDtoExtensions
{
    public static async Task<UserDto?> ToDtoAsync
    (
        this User user,
        UserVariant variant,
        IFileUrlGenerator fileUrlGenerator,
        CancellationToken cancellationToken = default
    )
    {
        if (variant == UserVariant.Empty)
        {
            return null;
        }

        var fileDtos = user.Files != null ? await Task.WhenAll(user.Files.Select(f => f.ToDtoAsync(fileUrlGenerator, cancellationToken))) : null;
        var profilePicture = fileDtos?.FirstOrDefault(f => f.Id == user.ProfilePicture.Id);
        return new UserDto
        (
            user.Id,
            user.Email.Value,
            user.UserName.Value,
            user.BirthDate.Value,
            user.Gender,
            user.DatingPreferences.ToDto(variant.DatingPreferencesVariant),
            user.Location?.ToDto(),
            profilePicture,
            fileDtos?.ToImmutableHashSet(),
            user.CreatedAt,
            user.UpdatedAt
        );
    }
}