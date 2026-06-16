namespace Embe.C2C.Application.Authorizations.FactStores.Messages.Facts;

public record AuthorMessageFact(Guid Id, bool Value) : AuthorizationFact(Id);