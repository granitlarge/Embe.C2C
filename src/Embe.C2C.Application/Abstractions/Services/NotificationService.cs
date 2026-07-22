using Embe.C2C.Application.Events;

namespace Embe.C2C.Application.Abstractions.Services;

/// <summary>
/// A notification service that sends the notification to an instance of our application running on the user's device.
/// Requires that the user is running our application.
/// </summary>
public interface IRealTimeNotificationService
{
    /// <summary>
    /// Sends a notification.
    /// </summary>
    /// <returns>True if the notification was delivered, else false.</returns>
    Task<bool> SendAsync<T>
    (
        T notification,
        CancellationToken cancellationToken = default
    ) where T : IntegrationEvent;
}

/// <summary>
/// A notification service that sends the notification to the user's device.
/// </summary>
public interface IDeviceNotificationService
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="endpoint">An opaque string that can be used to send the notification to the user's device.</param>
    /// <param name="notification">The notification</param>
    /// <param name="cancellationToken"></param>
    Task SendAsync<T>
    (
        string endpoint,
        T notification,
        CancellationToken cancellationToken = default
    ) where T : IntegrationEvent;
}