using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactStores.Conversations.Facts;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Authorizations.FactStores.Conversations;

public class ConversationAuthorizationFactStore
(
    IRepository repository,
    IAuthenticatedUserService authenticatedUser
) : AuthorizationFactStore(authenticatedUser)
{
    private readonly IRepository _repo = repository;

    public async ValueTask<IsParticipantConversationFact> GetIsParticipantFactAsync(Guid conversationId, CancellationToken cancellationToken)
    {
        var fact = GetFact<IsParticipantConversationFact>(conversationId);
        if (fact is not null)
        {
            return fact;
        }
        await LoadFactsAsync(conversationId, cancellationToken);
        return GetFact<IsParticipantConversationFact>(conversationId) ?? throw new InvalidOperationException($"IsParticipantConversationFact for conversation {conversationId} not found after loading facts.");
    }

    private async ValueTask LoadFactsAsync(Guid conversationId, CancellationToken cancellationToken)
    {
        var isParticipant = await _repo.DomainUsersQuery.AnyAsync(du =>
            du.Id == CurrentUserId &&
            (
                du.Matchings1!.Any(m => m.Conversation.Id == conversationId) ||
                du.Matchings2!.Any(m => m.Conversation.Id == conversationId)
            )
        , cancellationToken);
        var fact = new IsParticipantConversationFact(conversationId, isParticipant);
        SetFact(fact);
    }
}