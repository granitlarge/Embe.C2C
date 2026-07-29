using Embe.C2C.Application.Events.Notifications;
using Embe.C2C.Application.Services;

namespace Embe.C2C.Infrastructure.Browser;

public class NullDeviceNotificationService : IDeviceNotificationService
{
    public Task<bool> SendAsync<T>(T notification, CancellationToken cancellationToken = default) where T : NotificationIntegrationEvent
    {
        return Task.FromResult(false);
    }
}