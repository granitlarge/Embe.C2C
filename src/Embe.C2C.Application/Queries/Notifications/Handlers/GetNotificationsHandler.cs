using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Extensions;
using ErrorOr;

namespace Embe.C2C.Application.Queries.Notifications.Handlers;

public class GetNotificationsHandler
{
    private readonly NotificationAuthorizationService _notificationAuthorizationService;
    private readonly NotificationDtoMapper _notificationDtoMapper;
    private readonly INotificationRepository _notificationRepository;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public GetNotificationsHandler
    (
        INotificationRepository notificationRepository,
        IAuthenticatedUserService authenticatedUserService,
        NotificationAuthorizationService notificationAuthorizationService,
        NotificationDtoMapper notificationDtoMapper
    )
    {
        _authenticatedUserService = authenticatedUserService;
        _notificationRepository = notificationRepository;
        _notificationAuthorizationService = notificationAuthorizationService;
        _notificationDtoMapper = notificationDtoMapper;
    }

    public async Task<ErrorOr<List<ReadDto<NotificationDto, NotificationPermission>>>> HandleAsync(PagedQuery query, CancellationToken cancellationToken = default)
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("Authenticated user must have a user id.");
        var notifications = await _notificationRepository.GetNotificationsAsync(userId, query.Page, query.Size, cancellationToken);
        var dtos = notifications.Select(n => n.ToDto(_notificationAuthorizationService, _notificationDtoMapper)).ToList();
        return dtos;
    }
}