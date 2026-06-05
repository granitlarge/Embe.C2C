using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions.Services;

namespace Embe.C2C.Application.Authorizations;

internal class UserAuthorizationPolicy
{
    private readonly IUserService _userService;

    internal UserAuthorizationPolicy(IUserService userService)
    {
        _userService = userService;
    }

    public Guid GetActorId() => _userService.UserId ?? throw new InvalidOperationException("No user is currently authenticated.");

    public async Task<ImmutableHashSet<UserPermission>> GetPermissionsAsync(Guid targetUserId, CancellationToken cancellationToken = default)
    {
        var permissions = new HashSet<UserPermission>();
        if (_userService.UserId == targetUserId)
        {
            permissions.Add(UserPermission.Update);
            permissions.Add(UserPermission.Delete);
        }

        return [.. permissions];
    }
}

public enum UserPermission
{
    Update,
    Delete,
}