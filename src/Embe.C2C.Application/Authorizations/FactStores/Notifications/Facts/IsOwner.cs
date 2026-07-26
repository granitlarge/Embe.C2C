namespace Embe.C2C.Application.Authorizations.FactStores.Notifications.Facts;

public record IsOwner(Guid NotificationId, bool Value) : AuthorizationFact(NotificationId);