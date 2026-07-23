using Embe.C2C.Domain.Aggregates.Notifications;

namespace Embe.C2C.Application.Abstractions.Repos;

public interface INotificationRepository : IAggregateRepository<Notification, Guid>
{

}