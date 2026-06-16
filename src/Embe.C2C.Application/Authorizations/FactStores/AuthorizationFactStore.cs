using Embe.C2C.Application.Abstractions.Services;

namespace Embe.C2C.Application.Authorizations.FactStores;

public abstract class AuthorizationFactStore(IAuthenticatedUserService authenticatedUserService)
{
    public Guid CurrentUserId { get; } = authenticatedUserService.UserId ?? throw new InvalidOperationException("Authenticated user ID is not available.");

    private readonly Dictionary<Type, Dictionary<Guid, AuthorizationFact>> _facts = [];

    public T_Fact? GetFact<T_Fact>(Guid factId) where T_Fact : AuthorizationFact
    {
        if (_facts.TryGetValue(typeof(T_Fact), out var factDictionary) && factDictionary.TryGetValue(factId, out var fact))
        {
            return (T_Fact)fact;
        }
        return null;
    }

    public T_Fact SetFact<T_Fact>(T_Fact fact) where T_Fact : AuthorizationFact
    {
        if (!_facts.TryGetValue(typeof(T_Fact), out var factDictionary))
        {
            factDictionary = [];
            _facts[typeof(T_Fact)] = factDictionary;
        }
        factDictionary[fact.Id] = fact;
        return fact;
    }
}