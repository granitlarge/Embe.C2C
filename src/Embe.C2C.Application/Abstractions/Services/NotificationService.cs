using Embe.C2C.Application.Dtos.Read.Aggregates;

namespace Embe.C2C.Application.Abstractions.Services;

public interface INotificationService
{
    Task SendNotificationAsync<T>(T notification, CancellationToken cancellationToken = default) where T : NotificationDto;
}