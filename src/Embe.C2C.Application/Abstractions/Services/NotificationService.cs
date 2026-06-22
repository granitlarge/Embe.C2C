namespace Embe.C2C.Application.Abstractions.Services;

public interface INotificationService
{
    Task SendNotificationAsync<T>(T notification, CancellationToken cancellationToken = default);
}