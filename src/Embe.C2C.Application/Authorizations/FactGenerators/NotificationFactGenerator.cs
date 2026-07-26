using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactStores;

namespace Embe.C2C.Application.Authorizations.FactGenerators;

public class NotificationAuthorizationFactGenerator
(
    IAuthenticatedUserService authenticatedUserService,
    INotificationRepository notificationRepository
) : AuthorizationFactGenerator(authenticatedUserService)
{

    private readonly INotificationRepository _notificationRepository = notificationRepository;

    public Task<List<AuthorizationFact>> GetAllAuthorizationFactsAsync(Guid notificationId, CancellationToken cancellationToken)
    {
        return _notificationRepository.GetAllAuthorizationFactsAsync(CurrentUserId ?? Guid.Empty, notificationId, cancellationToken);
    }

}