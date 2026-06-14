using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Dtos.Read.Aggregates;

namespace Embe.C2C.Infrastructure.SignalR;

public class SignalRNotificationService : INotificationService
{
    public SignalRNotificationService()
    {

    }

    public Task SendNotificationAsync<T>(T notification, CancellationToken cancellationToken = default)
    where T : NotificationDto
    {
        throw new NotImplementedException();
    }
}