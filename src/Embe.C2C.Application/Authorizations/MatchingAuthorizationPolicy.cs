using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;

public class MatchingAuthorizationPolicy
{
    private readonly C2CContext _context;
    private readonly IUserService _userService;

    public MatchingAuthorizationPolicy(C2CContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<ImmutableHashSet<MatchingPermission>> GetPermissionsAsync
    (
        Guid matchingId,
        CancellationToken cancellationToken = default
    )
    {
        var userId = _userService.UserId ?? throw new InvalidOperationException("User is not authenticated.");

        var matching = await _context.Matchings.FindAsync([matchingId], cancellationToken);
        if (matching == null)
        {
            return [];
        }

        var permissions = new List<MatchingPermission>();
        if (matching.UserId1 == userId || matching.UserId2 == userId)
        {
            permissions.Add(MatchingPermission.Unmatch);
        }
        return [.. permissions];
    } 
}

public enum MatchingPermission
{
    Unmatch
}