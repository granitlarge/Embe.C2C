using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;

namespace Embe.C2C.Application.Queries.Notifications.Handlers;

public class HasUnreadHandler
(
    INotificationRepository notificationRepository, 
    IAuthenticatedUserService authenticatedUserService
)
{
    private readonly INotificationRepository _notificationsRepository = notificationRepository;
    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;

    public async Task<bool> HandleAsync(CancellationToken cancellationToken = default)
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("Authenticated user must have a user id.");
        var hasUnread = await _notificationsRepository.HasUnreadAsync(userId, cancellationToken);
        return hasUnread;
    }
}