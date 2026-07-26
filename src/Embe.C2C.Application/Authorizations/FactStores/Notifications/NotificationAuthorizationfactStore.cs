using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactGenerators;
using Embe.C2C.Application.Authorizations.FactStores.Notifications.Facts;
using Embe.C2C.Domain.Aggregates.Notifications;

namespace Embe.C2C.Application.Authorizations.FactStores.Notifications;

public class NotificationAuthorizationFactStore
(
    NotificationAuthorizationFactGenerator notificationFactGenerator,
    IAuthenticatedUserService authenticatedUserService
) : AuthorizationFactStore(authenticatedUserService)
{
    private readonly NotificationAuthorizationFactGenerator _notificationFactGenerator = notificationFactGenerator;

    public async Task<IsOwner> GetIsOwnerAsync(Guid notificationId, CancellationToken cancellationToken)
    {
        var fact = GetFact<IsOwner>(notificationId);
        if (fact is not null)
            return fact;

        await LoadAllFactsAsync(notificationId, cancellationToken);
        return GetFact<IsOwner>(notificationId) ?? throw new InvalidOperationException("Fact not found after loading all facts");
    }

    internal IsOwner GetIsOwner(Notification notification)
    {
        return new IsOwner(notification.Id, notification.RecipientUserId == CurrentUserId);
    }

    private async Task LoadAllFactsAsync(Guid notificationId, CancellationToken cancellationToken)
    {
        var facts = await _notificationFactGenerator.GetAllAuthorizationFactsAsync(notificationId, cancellationToken);
        foreach (var fact in facts)
        {
            SetFact(fact);
        }
    }

}