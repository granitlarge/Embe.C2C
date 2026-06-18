using Embe.C2C.Application.Abstractions.Services;

namespace Embe.C2C.Application.Authorizations.FactStores;

public abstract class AuthorizationFactStore(IAuthenticatedUserService authenticatedUserService)
{
    public Guid CurrentUserId { get; } = authenticatedUserService.UserId ?? throw new InvalidOperationException("Authenticated user ID is not available.");

    private readonly Dictionary<Type, Dictionary<Guid, AuthorizationFact>> _facts = [];

    public T_Fact? GetFact<T_Fact>(Guid factId) where T_Fact : AuthorizationFact
    {
        var factType = typeof(T_Fact);
        if (_facts.TryGetValue(factType, out var factDictionary) && factDictionary.TryGetValue(factId, out var fact))
        {
            return (T_Fact)fact;
        }
        return null;
    }

    public T_Fact SetFact<T_Fact>(T_Fact fact) where T_Fact : AuthorizationFact
    {
        var factType = fact.GetType();
        if (!_facts.TryGetValue(factType, out var factDictionary))
        {
            factDictionary = [];
            _facts[factType] = factDictionary;
        }
        factDictionary[fact.Id] = fact;
        return fact;
    }
}