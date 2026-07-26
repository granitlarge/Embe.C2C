using Embe.C2C.Application.Authorizations.FactStores;
using Embe.C2C.Domain.Aggregates.Notifications;

namespace Embe.C2C.Application.Abstractions.Repos;

public interface INotificationRepository : IGenericRepository<Notification, Guid>
{
    Task<bool> HasUnreadAsync(Guid userId, CancellationToken cancellationToken);

    Task<List<Notification>> GetNotificationsAsync(Guid recipientUserId, int page, int pageSize, CancellationToken cancellationToken);
    Task<List<AuthorizationFact>> GetAllAuthorizationFactsAsync(Guid currentUser, Guid notificationId, CancellationToken cancellationToken);
}