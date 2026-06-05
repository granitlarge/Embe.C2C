using Embe.C2C.Domain.Aggregates.Notifications;

namespace Embe.C2C.Application.Abstractions.Services;

public interface INotificationService
{
    Task SendNotificationAsync(Notification notification, CancellationToken cancellationToken = default);
}