using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactStores;
using Embe.C2C.Application.Authorizations.FactStores.Conversations.Facts;
using Embe.C2C.Application.Authorizations.FactStores.Messages.Facts;
using Embe.C2C.Domain.Aggregates.Messages;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Authorizations.FactGenerators;

public class MessageFactGenerator
(
    IRepository repo,
    IAuthenticatedUserService authenticatedUserService
) : AuthorizationFactGenerator(authenticatedUserService)
{
    private readonly IRepository _repo = repo;

    private AuthorMessageFact GetAuthorFact(Guid messageId, Guid messageAuthorUserId)
    {
        var fact = new AuthorMessageFact(messageId, messageAuthorUserId == CurrentUserId);
        return fact;
    }

    private RecipientMessageFact GetRecipientFact
    (
        Guid messageId,
        Guid messageAuthorUserId,
        Guid? messageRecipientUserId = null,
        bool? isConversationParticipant = null
    )
    {
        if (messageRecipientUserId is null && isConversationParticipant is null)
        {
            throw new ArgumentException("Either messageRecipientUserId or isConversationParticipant must be provided.");
        }

        var isRecipient = isConversationParticipant.HasValue
            ? isConversationParticipant.Value && messageAuthorUserId != CurrentUserId
            : (messageRecipientUserId == CurrentUserId) && messageAuthorUserId != CurrentUserId;

        var fact = new RecipientMessageFact(messageId, isRecipient);
        return fact;
    }

    public RecipientMessageFact GetRecipientFact(Message message, IsParticipantConversationFact isParticipantFact)
    {
        if (isParticipantFact.ConversationId != message.ConversationId)
        {
            throw new ArgumentException($"isParticipantFact ConversationId {isParticipantFact.ConversationId} does not match message's ConversationId {message.ConversationId}.");
        }

        return GetRecipientFact(message.Id, message.AuthorUserId, null, isParticipantFact.Value);
    }

    public async Task<RecipientMessageFact> GetRecipientFactAsync(Message message)
    {
        var facts = await GetAllFactsAsync(message.Id);
        var recipientFact = facts.OfType<RecipientMessageFact>().Single();
        return recipientFact;
    }

    public AuthorMessageFact GetAuthorFact(Message message)
    {
        return GetAuthorFact(message.Id, message.AuthorUserId);
    }

    public async Task<AuthorizationFact[]> GetAllFactsAsync
    (
        Guid messageId,
        CancellationToken cancellationToken = default
    )
    {
        var facts = await LoadFactsAsync(messageId, cancellationToken);
        return facts;
    }

    private async Task<AuthorizationFact[]> LoadFactsAsync(Guid messageId, CancellationToken cancellationToken)
    {
        var facts = await _repo
            .MessagesQuery
            .Where(m => m.Id == messageId)
            .Select(m => new
            {
                m.AuthorUserId,
                RecipientUserId = m.Conversation!.UserId1 != m.AuthorUserId ? m.Conversation.UserId1 : m.Conversation.UserId2
            })
            .SingleOrDefaultAsync(cancellationToken);

        AuthorMessageFact authorMessageFact;
        RecipientMessageFact recipientMessageFact;
        if (facts is null)
        {
            authorMessageFact = new AuthorMessageFact(messageId, false);
            recipientMessageFact = new RecipientMessageFact(messageId, false);
        }
        else
        {
            authorMessageFact = GetAuthorFact(messageId, facts.AuthorUserId);
            recipientMessageFact = GetRecipientFact(messageId, facts.AuthorUserId, facts.RecipientUserId);
        }
        return [authorMessageFact, recipientMessageFact];
    }
}