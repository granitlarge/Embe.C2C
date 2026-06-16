using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactStores.Conversations;
using Embe.C2C.Application.Authorizations.FactStores.Conversations.Facts;
using Embe.C2C.Application.Authorizations.FactStores.Messages.Facts;
using Embe.C2C.Domain.Aggregates.Messages;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Authorizations.FactStores.Messages;

public class MessageAuthorizationFactStore
(
    IRepository repo,
    IAuthenticatedUserService authenticatedUserService,
    ConversationAuthorizationFactStore conversationFactStore
) : AuthorizationFactStore(authenticatedUserService)
{
    private readonly IRepository _repo = repo;
    private readonly ConversationAuthorizationFactStore _conversationFactStore = conversationFactStore;

    public AuthorMessageFact GetAuthorFact(Message message)
    {
        var fact = GetFact<AuthorMessageFact>(message.Id);
        if (fact is not null)
        {
            return fact;
        }
        return SetFact(new AuthorMessageFact(message.Id, message.AuthorUserId == CurrentUserId));
    }

    public async ValueTask<RecipientMessageFact> GetRecipientFactAsync(Message message, CancellationToken cancellationToken)
    {
        var fact = GetFact<RecipientMessageFact>(message.Id);
        if (fact is not null)
        {
            return fact;
        }

        var conversationFact = await _conversationFactStore.GetIsParticipantFactAsync(message.ConversationId, cancellationToken);
        return SetFact(new RecipientMessageFact(message.Id, conversationFact.Value && message.AuthorUserId != CurrentUserId));
    }

    public async ValueTask<AuthorMessageFact> GetAuthorFactAsync(Guid messageId, CancellationToken cancellationToken)
    {
        var fact = GetFact<AuthorMessageFact>(messageId);
        if (fact is not null)
        {
            return fact;
        }
        await LoadFactsAsync(messageId, cancellationToken);
        return GetFact<AuthorMessageFact>(messageId) ?? throw new InvalidOperationException($"AuthorMessageFact for message {messageId} not found after loading facts.");
    }

    public async ValueTask<RecipientMessageFact> GetRecipientFactAsync(Guid messageId, CancellationToken cancellationToken)
    {
        var fact = GetFact<RecipientMessageFact>(messageId);
        if (fact is not null)
        {
            return fact;
        }
        await LoadFactsAsync(messageId, cancellationToken);
        return GetFact<RecipientMessageFact>(messageId) ?? throw new InvalidOperationException($"RecipientMessageFact for message {messageId} not found after loading facts.");
    }

    private async ValueTask LoadFactsAsync(Guid messageId, CancellationToken cancellationToken)
    {
        var facts = await _repo.MessagesQuery
            .Where(m => m.Id == messageId)
            .Select(m => new
            {
                IsAuthor = m.AuthorUserId == CurrentUserId,
                IsRecipient = m.Conversation!.UserId1 == CurrentUserId || m.Conversation.UserId2 == CurrentUserId,
                m.ConversationId
            })
            .SingleOrDefaultAsync(cancellationToken);

        AuthorMessageFact authorMessageFact;
        RecipientMessageFact recipientMessageFact;
        IsParticipantConversationFact? isParticipantConversationFact = null;
        if (facts is null)
        {
            authorMessageFact = new AuthorMessageFact(messageId, false);
            recipientMessageFact = new RecipientMessageFact(messageId, false);
        }
        else
        {
            authorMessageFact = new AuthorMessageFact(messageId, facts.IsAuthor);
            recipientMessageFact = new RecipientMessageFact(messageId, !facts.IsAuthor && facts.IsRecipient);
            isParticipantConversationFact = new IsParticipantConversationFact(facts.ConversationId, facts.IsRecipient);
        }

        SetFact(authorMessageFact);
        SetFact(recipientMessageFact);
        if (isParticipantConversationFact is not null)
            SetFact(isParticipantConversationFact);
    }
}