namespace Embe.C2C.Application.Authorizations.Contexts;

public class AuthorizationContext
{
    private readonly Dictionary<Type, Dictionary<Guid, Fact>> _factsCache = [];

    public T? Get<T>(Guid id)
        where T : Fact
    {
        var type = typeof(T);
        if (_factsCache.TryGetValue(type, out var cachedFacts) && cachedFacts.TryGetValue(id, out var fact))
        {
            return (T)fact;
        }
        return null;
    }

    public void Store<T>(T fact)
        where T : Fact
    {
        var type = typeof(T);
        if (_factsCache.TryGetValue(type, out var cachedFacts))
        {
            cachedFacts[fact.Id] = fact;
        }
        else
        {
            _factsCache[type] = new Dictionary<Guid, Fact> { { fact.Id, fact } };
        }
    }
}

public abstract record Fact(Guid Id);
public record UserFact(Guid UserId, bool IsBlockedBy, bool IsBlocking, bool IsMatched, bool IsSame) : Fact(UserId);
public record MatchFact(Guid MatchId, bool IsParticipant) : Fact(MatchId);
public record ConversationFact(Guid ConversationId, bool IsParticipant) : Fact(ConversationId);
public record MessageFact(Guid MessageId, bool IsAuthor, bool IsRecipient) : Fact(MessageId);
public record JudgementFact(Guid JudgeeId, bool CanJudge) : Fact(JudgeeId);