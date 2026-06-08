using Embe.C2C.Application.Abstractions.Services;

namespace Embe.C2C.Infrastructure.SignalR;

public class SignalRNotificationService : INotificationService
{
    public SignalRNotificationService()
    {

    }

    public Task SendNotificationAsync<T>(T notification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}