namespace Embe.C2C.Application.Authorizations.FactStores.Conversations.Facts;

public record IsParticipantConversationFact(Guid ConversationId, bool Value) : AuthorizationFact(ConversationId);