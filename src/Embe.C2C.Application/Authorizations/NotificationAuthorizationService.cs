using System.Collections.Immutable;
using Embe.C2C.Application.Authorizations.FactStores.Notifications;
using Embe.C2C.Application.Authorizations.FactStores.Notifications.Facts;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Notifications;

namespace Embe.C2C.Application.Authorizations;

public class NotificationAuthorizationService(NotificationAuthorizationFactStore notificationAuthorizationFactStore)
{
    private readonly NotificationAuthorizationFactStore _notificationAuthorizationFactStore = notificationAuthorizationFactStore;

    public async Task<(ImmutableHashSet<NotificationPermission> Permissions, NotificationVariant Variant)> GetAsync
    (
        Guid notificationId,
        CancellationToken cancellationToken
    )
    {
        var isOwner = await _notificationAuthorizationFactStore.GetIsOwnerAsync(notificationId, cancellationToken);
        var permissions = GetPermissions(isOwner);
        var variant = GetVariant(isOwner);
        return (permissions, variant);
    }

    public (ImmutableHashSet<NotificationPermission> Permissions, NotificationVariant Variant) Get
    (
        Notification notification
    )
    {
        var isOwner = _notificationAuthorizationFactStore.GetIsOwner(notification);
        var permissions = GetPermissions(isOwner);
        var variant = GetVariant(isOwner);
        return (permissions, variant);
    }

    private static ImmutableHashSet<NotificationPermission> GetPermissions(IsOwner isOwner)
    {
        var permissions = new HashSet<NotificationPermission>();
        if (isOwner.Value)
        {
            permissions.Add(NotificationPermission.View);
            permissions.Add(NotificationPermission.Delete);
            permissions.Add(NotificationPermission.MarkAsRead);
        }
        return [.. permissions];
    }

    private static NotificationVariant GetVariant(IsOwner isOwner)
    {
        if (isOwner.Value)
        {
            return NotificationVariant.Full;
        }
        return NotificationVariant.Empty;
    }
}

public enum NotificationPermission
{
    View = 1,
    Delete = 2,
    MarkAsRead = 3
}