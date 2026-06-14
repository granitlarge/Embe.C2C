using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Notifications.Handlers;

public class GetNotificationsHandler
{
    private readonly IRepository _context;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public GetNotificationsHandler(IRepository context, IAuthenticatedUserService authenticatedUserService)
    {
        _context = context;
        _authenticatedUserService = authenticatedUserService;
    }

    public async Task<Result<List<NotificationDto>>> HandleAsync(PagedQuery query, CancellationToken cancellationToken = default)
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("Authenticated user must have a user id.");
        var notifications = await _context.NotificationsQuery
            .AsNoTracking()
            .Where(n => n.RecipientUserId == userId)
            .Skip((query.Page - 1) * query.Size)
            .Take(query.Size)
            .ToListAsync(cancellationToken);
        var dtos = notifications.Select(n => n.ToDto()).ToList();
        return Result<List<NotificationDto>>.Success(dtos);
    }
}