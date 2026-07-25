using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactGenerators;
using Embe.C2C.Application.Authorizations.FactStores.Matches;
using Embe.C2C.Application.Authorizations.FactStores.Messages.Facts;
using Embe.C2C.Domain.Aggregates.Messages;
using Embe.C2C.Domain.Errors.Aggregates;

namespace Embe.C2C.Application.Authorizations.FactStores.Messages;

public class MessageAuthorizationFactStore
(
    MatchingAuthorizationFactStore matchingAuthorizationFactStore,
    MessageFactGenerator messageFactGenerator,
    IAuthenticatedUserService authenticatedUserService
) : AuthorizationFactStore(authenticatedUserService)
{
    private readonly MatchingAuthorizationFactStore _matchingAuthorizationFactStore = matchingAuthorizationFactStore;
    private readonly MessageFactGenerator _messageFactGenerator = messageFactGenerator;

    public AuthorMessageFact GetAuthorFact(Message message)
    {
        var fact = GetFact<AuthorMessageFact>(message.Id);
        if (fact is not null)
        {
            return fact;
        }
        return SetFact(_messageFactGenerator.GetAuthorFact(message));
    }

    public async ValueTask<RecipientMessageFact> GetRecipientFactAsync(Message message, CancellationToken cancellationToken)
    {
        var fact = GetFact<RecipientMessageFact>(message.Id);
        if (fact is not null)
        {
            return fact;
        }

        var isMatchedFact = await _matchingAuthorizationFactStore.GetIsParticipantFactAsync(message.MatchingId, cancellationToken);
        fact = _messageFactGenerator.GetRecipientFact(message, isMatchedFact);
        return SetFact(fact);
    }

    public async ValueTask<AuthorMessageFact> GetAuthorFactAsync(Guid messageId, CancellationToken cancellationToken)
    {
        var fact = GetFact<AuthorMessageFact>(messageId);
        if (fact is not null)
        {
            return fact;
        }
        var facts = await _messageFactGenerator.GetAllFactsAsync(messageId, cancellationToken);
        foreach (var f in facts)
        {
            SetFact(f);
        }
        return GetFact<AuthorMessageFact>(messageId) ?? throw new InvalidOperationException($"AuthorMessageFact for message {messageId} not found after loading facts.");
    }

    public async ValueTask<RecipientMessageFact> GetRecipientFactAsync(Guid messageId, CancellationToken cancellationToken)
    {
        var fact = GetFact<RecipientMessageFact>(messageId);
        if (fact is not null)
        {
            return fact;
        }
        var facts = await _messageFactGenerator.GetAllFactsAsync(messageId, cancellationToken);
        foreach (var f in facts)
        {
            SetFact(f);
        }
        return GetFact<RecipientMessageFact>(messageId) ?? throw new InvalidOperationException($"RecipientMessageFact for message {messageId} not found after loading facts.");
    }
}