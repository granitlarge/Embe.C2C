using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.Contexts;
using Embe.C2C.Application.Dtos;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Authorizations;

public class UserAuthorizationPolicy
{
    private readonly AuthorizationContext _context;
    private readonly IRepository _repo;
    private readonly IFileUrlGenerator _fileUrlGenerator;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public UserAuthorizationPolicy
    (
        AuthorizationContext context,
        IFileService fileService,
        IRepository repo,
        IAuthenticatedUserService authenticatedUserService
    )
    {
        _context = context;
        _fileUrlGenerator = new FileUrlGenerator(fileService, TimeSpan.FromSeconds(15));
        _repo = repo;
        _authenticatedUserService = authenticatedUserService;
    }

    public async Task<ReadDto<UserDto, UserPermission>?> ToDtoAsync
    (
        User user,
        CancellationToken cancellationToken = default
    )
    {
        var (permissions, variant) = await GetAsync(user.Id, cancellationToken);

        var dto = await user.ToDtoAsync(variant, _fileUrlGenerator, cancellationToken);
        if (dto is null)
            return null;

        return new ReadDto<UserDto, UserPermission>
        (
            dto,
            permissions
        );
    }

    public async ValueTask<(ImmutableHashSet<UserPermission> Permissions, UserVariant Variant)> GetAsync
    (
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var fact = await GetUserFactAsync(userId, cancellationToken);
        var permissions = GetPermissions(fact);
        var variant = GetVariant(fact);
        return (permissions, variant);
    }

    private static UserVariant GetVariant(UserFact fact)
    {
        if (fact.IsSame)
            return UserVariant.Full;

        if (fact.IsMatched)
            return UserVariant.Matched;

        return UserVariant.Empty;
    }

    private static ImmutableHashSet<UserPermission> GetPermissions
    (
        UserFact fact
    )
    {
        if (fact.IsBlockedBy || fact.IsBlocking)
        {
            return [];
        }

        var permissions = new HashSet<UserPermission>();
        if (fact.IsSame)
        {
            permissions.Add(UserPermission.View);
            permissions.Add(UserPermission.Update);
            permissions.Add(UserPermission.Delete);
        }

        if (fact.IsMatched)
        {
            permissions.Add(UserPermission.View);
        }

        return [.. permissions];
    }

    private async ValueTask<UserFact> GetUserFactAsync(Guid otherUserId, CancellationToken cancellationToken = default)
    {
        var userId = _authenticatedUserService.UserId;

        if (_context.Get<UserFact>(otherUserId) is UserFact cachedFact)
        {
            return cachedFact;
        }

        if (userId == otherUserId)
        {
            var sameFact = new UserFact
            (
                UserId: userId.Value,
                IsBlockedBy: false,
                IsBlocking: false,
                IsMatched: false,
                IsSame: true
            );

            _context.Store(sameFact);
            return sameFact;
        }

        var fact = await _repo.DomainUsersQuery
            .Where(u => u.Id == otherUserId)
            .Select(u => new UserFact
            (
                UserId: otherUserId,
                IsBlockedBy: u.Blocked!.Any(b => b.BlockedUserId == userId),
                IsBlocking: u.BlockedBy!.Any(b => b.BlockerUserId == userId),
                IsMatched: u.Matchings1!.Any(m => m.UserId1 == userId || m.UserId2 == userId) || u.Matchings2!.Any(m => m.UserId1 == userId || m.UserId2 == userId),
                IsSame: u.Id == userId
            ))
            .FirstOrDefaultAsync(cancellationToken) ?? new UserFact
            (
                UserId: otherUserId,
                IsBlockedBy: false,
                IsBlocking: false,
                IsMatched: false,
                IsSame: false
            );

        _context.Store(fact);
        return fact;
    }

}

public enum UserPermission
{
    View = 0,
    Update = 1,
    Delete = 2
}