using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;

namespace Embe.C2C.Application.Extensions.Domain.Aggregates;

public static class UserExtensions
{
    public static async Task<ReadDto<UserDto, UserPermission>?> ToDtoAsync
    (
        this C2C.Domain.Aggregates.Users.User user,
        UserAuthorizationService userAuthorizationService,
        UserDtoMapper userDtoMapper,
        CancellationToken cancellationToken = default
    )
    {
        var (permissions, variant) = await userAuthorizationService.GetAsync(user.Id, cancellationToken);
        var dto = await userDtoMapper.ToDtoAsync(user, variant, cancellationToken);
        if (dto is null)
            return null;

        return new ReadDto<UserDto, UserPermission>
        (
            dto,
            permissions
        );
    }
}