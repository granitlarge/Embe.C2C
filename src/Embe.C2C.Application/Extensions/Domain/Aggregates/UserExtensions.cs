using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Enrichment.Aggregates;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Errors.Aggregates;

namespace Embe.C2C.Application.Extensions.Domain.Aggregates;

public static class UserExtensions
{
    public static UserEnriched Enrich(this User user, User? queryingUser)
    {
        var distanceKm = user.Location != null && queryingUser?.Location != null
            ? user.Location.DistanceTo(queryingUser.Location).ToKilometers().Value
            : (double?)null;
        return new UserEnriched(user, distanceKm);
    }
}

public static class EnrichedUserExtensions
{
    public static async Task<ReadDto<UserDto, UserPermission>?> ToDtoAsync
    (
        this UserEnriched enrichedUser,
        UserAuthorizationService userAuthorizationService,
        UserDtoMapper userDtoMapper,
        CancellationToken cancellationToken = default
    )
    {
        var user = enrichedUser.User;
        var (permissions, variant) = await userAuthorizationService.GetAsync(user.Id, cancellationToken);
        var dto = await userDtoMapper.ToDtoAsync(enrichedUser, variant, cancellationToken);
        if (dto is null)
            return null;

        return new ReadDto<UserDto, UserPermission>
        (
            dto,
            permissions
        );

    }
}