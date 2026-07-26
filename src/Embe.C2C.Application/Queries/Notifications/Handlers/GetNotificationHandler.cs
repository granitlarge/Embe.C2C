using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.Extensions;
using ErrorOr;

namespace Embe.C2C.Application.Queries.Notifications.Handlers;

public class GetNotificationHandler
(
    NotificationDtoMapper notificationDtoMapper,
    NotificationAuthorizationService notificationAuthorizationService,
    INotificationRepository notificationRepository,
    IRepository repository
) : TransactionalQueryHandler<GetNotificationQuery, ErrorOr<ReadDto<NotificationDto, NotificationPermission>>>(repository)
{

    private readonly INotificationRepository _notificationRepository = notificationRepository;
    private readonly NotificationDtoMapper _notificationDtoMapper = notificationDtoMapper;
    private readonly NotificationAuthorizationService _notificationAuthorizationService = notificationAuthorizationService;

    protected override async Task<ErrorOr<ReadDto<NotificationDto, NotificationPermission>>> ExecuteAsync(GetNotificationQuery query, CancellationToken cancellationToken = default)
    {
        var (permissions, _) = await _notificationAuthorizationService.GetAsync(query.NotificationId, cancellationToken);
        if (!permissions.Contains(NotificationPermission.View))
        {
            return ApplicationErrors.Forbidden.ToForbiddenErrorOr();
        }

        var notification = await _notificationRepository.GetByIdAsync(query.NotificationId, cancellationToken);
        if (notification is null)
        {
            return ApplicationErrors.NotFound.ToNotFoundErrorOr();
        }

        var dto = notification.ToDto(_notificationAuthorizationService, _notificationDtoMapper);
        if (dto is null)
        {
            return ApplicationErrors.Forbidden.ToForbiddenErrorOr();
        }

        return dto;
    }
}