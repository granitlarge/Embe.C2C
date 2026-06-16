namespace Embe.C2C.Application.Authorizations.FactStores.Messages.Facts;

public record RecipientMessageFact(Guid Id, bool Value) : AuthorizationFact(Id);