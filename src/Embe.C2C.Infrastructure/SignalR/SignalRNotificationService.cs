using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Domain.Aggregates.Notifications;

namespace Embe.C2C.Infrastructure.SignalR;

public class SignalRNotificationService : INotificationService
{
    public SignalRNotificationService()
    {

    }

    public Task SendNotificationAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}