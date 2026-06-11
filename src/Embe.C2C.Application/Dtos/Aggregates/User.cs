using System.Collections.Immutable;
using Embe.C2C.Application.Dtos.Entities;
using Embe.C2C.Application.Dtos.ValueObjects;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Dtos.Aggregates;

public record UserDto
(
    Guid Id,
    string Email,
    string UserName,
    DateOnly BirthDate,
    Gender Gender,
    DatingPreferencesDto DatingPreferences,
    LocationDto? Location,
    ImmutableHashSet<FileDto> Files,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

public record UserBriefDto
(
    Guid Id,
    string UserName,
    string ProfilePictureUrl
);

public static class UserDtoExtensions
{
    public static async Task<UserDto> ToDtoAsync(this User user, IFileUrlGenerator fileUrlGenerator, CancellationToken cancellationToken = default)
    {
        return new UserDto
        (
            user.Id,
            user.Email.Value,
            user.UserName.Value,
            user.BirthDate.Value,
            user.Gender,
            user.DatingPreferences.ToDto(),
            user.Location?.ToDto(),
            [.. await Task.WhenAll(user.Files.Select(f => f.ToDtoAsync(fileUrlGenerator, cancellationToken)))],
            user.CreatedAt,
            user.UpdatedAt
        );
    }

    public static async Task<UserBriefDto> ToBriefDtoAsync(this User user, IFileUrlGenerator fileUrlGenerator, CancellationToken cancellationToken)
    {
        var profilePictureFileName = user.ProfilePicture.FileDetails.Name;
        var profilePictureUrl = await fileUrlGenerator.GenerateUrlAsync(profilePictureFileName, cancellationToken);

        return new UserBriefDto
        (
            user.Id,
            user.UserName.Value,
            profilePictureUrl
        );
    }
}