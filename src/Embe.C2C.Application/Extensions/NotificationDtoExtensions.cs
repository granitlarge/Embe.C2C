using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Domain.Aggregates.Notifications;

namespace Embe.C2C.Application.Extensions;

public static class NotificationDtoExtensions
{
    public static ReadDto<NotificationDto, NotificationPermission> ToDto<T>
    (
        this T notification,
        NotificationAuthorizationService authorizationService,
        NotificationDtoMapper dtoMapper
    ) where T : Notification
    {
        var (permissions, variant) = authorizationService.Get
        (
            notification
        );

        var dto = dtoMapper.ToDto(notification, variant);
        return new ReadDto<NotificationDto, NotificationPermission>(dto, permissions);
    }
}
