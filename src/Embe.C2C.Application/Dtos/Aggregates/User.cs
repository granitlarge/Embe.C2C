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
    LocationDto Location,
    ImmutableHashSet<FileDto> Files,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

public static class UserDtoExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        (
            user.Id,
            user.Email.Value,
            user.UserName.Value,
            user.BirthDate.Value,
            user.Gender,
            user.DatingPreferences.ToDto(),
            user.Location.ToDto(),
            [.. user.Files.Select(f => f.ToDto())],
            user.CreatedAt,
            user.UpdatedAt
        );
    }
}