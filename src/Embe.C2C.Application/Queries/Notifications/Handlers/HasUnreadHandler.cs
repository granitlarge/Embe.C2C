using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Notifications.Handlers;

public class HasUnreadHandler
{
    private readonly IRepository _context;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public HasUnreadHandler(IRepository context, IAuthenticatedUserService authenticatedUserService)
    {
        _context = context;
        _authenticatedUserService = authenticatedUserService;
    }

    public async Task<Result<bool>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("Authenticated user must have a user id.");
        var hasUnread = await _context.NotificationsQuery
            .AsNoTracking()
            .AnyAsync(n => n.RecipientUserId == userId && n.ReadAt == null, cancellationToken);
        return Result<bool>.Success(hasUnread);
    }
}