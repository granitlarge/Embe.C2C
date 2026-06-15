namespace Embe.C2C.Application.Authorizations.Contexts;

public class AuthorizationContext
{
    private readonly Dictionary<Type, Dictionary<Guid, AuthorizationFact>> _factsCache = [];

    public T? Get<T>(Guid id)
        where T : AuthorizationFact
    {
        var type = typeof(T);
        if (_factsCache.TryGetValue(type, out var cachedFacts) && cachedFacts.TryGetValue(id, out var fact))
        {
            return (T)fact;
        }
        return null;
    }

    public void Store<T>(T fact)
        where T : AuthorizationFact
    {
        var type = typeof(T);
        if (_factsCache.TryGetValue(type, out var cachedFacts))
        {
            cachedFacts[fact.Id] = fact;
        }
        else
        {
            _factsCache[type] = new Dictionary<Guid, AuthorizationFact> { { fact.Id, fact } };
        }
    }
}

public abstract record AuthorizationFact(Guid Id);
public record UserFact(Guid UserId, bool IsBlockedBy, bool IsBlocking, bool IsMatched, bool IsSame) : AuthorizationFact(UserId);
public record MatchFact(Guid MatchId, bool IsParticipant) : AuthorizationFact(MatchId);
public record ConversationFact(Guid ConversationId, bool IsParticipant) : AuthorizationFact(ConversationId);
public record MessageFact(Guid MessageId, bool IsAuthor, bool IsRecipient) : AuthorizationFact(MessageId);
public record JudgementFact(Guid JudgeeId, bool CanJudge) : AuthorizationFact(JudgeeId);