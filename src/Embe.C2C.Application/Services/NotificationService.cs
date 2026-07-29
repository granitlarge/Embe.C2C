using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Events;
using Embe.C2C.Application.Events.Notifications;

namespace Embe.C2C.Application.Services;

/// <summary>
/// Delivers updates to the user in real-time.
/// </summary>
public interface IRealTimeUpdateService
{
    Task SendAsync<T>(T update, CancellationToken cancellationToken = default) where T : IntegrationEvent;
}

/// <summary>
/// Delivers notifications to the user in real-time.
/// </summary>
public interface IRealTimeNotificationService
{
    Task<bool> SendAsync<T>(T notification, CancellationToken cancellationToken = default) where T : NotificationIntegrationEvent;
}

/// <summary>
/// Delivers notifications to the user's device.
/// </summary>
public interface IDeviceNotificationService
{
    Task<bool> SendAsync<T>(T notification, CancellationToken cancellationToken = default) where T : NotificationIntegrationEvent;
}

public interface INotificationService
{
    Task SendAsync<T>(T notification, CancellationToken cancellationToken) where T : NotificationIntegrationEvent;
}

public class NotificationService
(
    IRealTimeNotificationService realTime,
    IDeviceNotificationService device,
    IEmailService email,
    EmailComposerService emailComposer,
    IUserRepository userRepository
) : INotificationService
{
    private readonly IRealTimeNotificationService _realTime = realTime;
    private readonly IDeviceNotificationService _device = device;
    private readonly IEmailService _email = email;
    private readonly EmailComposerService _emailComposer = emailComposer;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task SendAsync<T>
    (
        T notification,
        CancellationToken cancellationToken
    ) where T : NotificationIntegrationEvent
    {
        if (notification is not NotificationCreatedIntegrationEvent created)
        {
            return;
        }

        var delivered = await _realTime.SendAsync(created, cancellationToken);
        if (delivered)
            return;

        delivered = await _device.SendAsync(created, cancellationToken);
        if (delivered)
            return;

        await SendEmailAsync(created, cancellationToken);
    }

    private async Task SendEmailAsync(NotificationCreatedIntegrationEvent created, CancellationToken cancellationToken)
    {

#warning before sending an e-mail notification, we should ensure that the user hasn't disabled e-mail notifications

        var userEmail = (await _userRepository.GetByIdAsync(created.RecipientUserId, cancellationToken))?.Email.Value;
        if (userEmail is null)
            return;

        var (Subject, HtmlContent, PlainText) = await _emailComposer.CreateMessageAsync(created, cancellationToken);
        await _email.SendAsync(userEmail, Subject, HtmlContent, PlainText, cancellationToken);

    }
}