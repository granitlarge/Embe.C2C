namespace Embe.C2C.Application.Authorizations.Contexts;

public class AuthorizationContext
{
    private readonly Dictionary<Type, List<object>> _factsCache = [];

    public IReadOnlyList<T> Get<T>()
        where T : Fact
    {
        var type = typeof(T);
        if (_factsCache.TryGetValue(type, out var cachedFacts))
        {
            return [.. cachedFacts.Cast<T>()];
        }
        return [];
    }

    public void Store<T>(T fact)
        where T : Fact
    {
        var type = typeof(T);
        if (_factsCache.TryGetValue(type, out var cachedFacts))
        {
            cachedFacts.Add(fact);
        }
        else
        {
            _factsCache[type] = [fact];
        }
    }
}

public abstract record Fact();
public record UserFact(Guid UserId, bool IsBlockedBy, bool IsBlocking, bool IsMatched, bool IsSame) : Fact();
public record MatchFact(Guid MatchId, bool IsParticipant) : Fact();
public record ConversationFact(Guid ConversationId, bool IsParticipant) : Fact();
public record MessageFact(Guid MessageId, bool IsAuthor, bool IsRecipient) : Fact();
public record JudgementFact(Guid JudgeeId, bool CanJudge) : Fact();