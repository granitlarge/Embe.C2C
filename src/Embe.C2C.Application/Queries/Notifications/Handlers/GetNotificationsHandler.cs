using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Extensions;
using ErrorOr;

namespace Embe.C2C.Application.Queries.Notifications.Handlers;

public class GetNotificationsHandler
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IRepository _context;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public GetNotificationsHandler(INotificationRepository notificationRepository, IRepository context, IAuthenticatedUserService authenticatedUserService)
    {
        _context = context;
        _authenticatedUserService = authenticatedUserService;
        _notificationRepository = notificationRepository;
    }

    public async Task<ErrorOr<List<NotificationDto>>> HandleAsync(PagedQuery query, CancellationToken cancellationToken = default)
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("Authenticated user must have a user id.");
        var notifications = await _notificationRepository.GetNotificationsAsync(userId, query.Page, query.Size, cancellationToken);
        var dtos = notifications.Select(n => n.ToDto()).ToList();
        return dtos;
    }
}