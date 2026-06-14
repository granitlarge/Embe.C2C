using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.Contexts;

namespace Embe.C2C.Application.Authorizations;

public class JudgementAuthorizationPolicy
{
    private readonly IAuthenticatedUserService _user;
    private readonly IRepository _repo;
    private readonly AuthorizationContext _authorizationContext;

    public JudgementAuthorizationPolicy
    (
        IAuthenticatedUserService user,
        IRepository repo,
        AuthorizationContext authorizationContext
    )
    {
        _user = user;
        _repo = repo;
        _authorizationContext = authorizationContext;
    }

    private static readonly ImmutableHashSet<JudgementPermission> JudgePermissions = [JudgementPermission.Judge];

    public async Task<ImmutableHashSet<JudgementPermission>> GetPermissionsAsync(Guid judgeeUserId, CancellationToken cancellationToken)
    {
        var facts = await GetJudgementFactAsync(judgeeUserId, cancellationToken);
        if (facts.CanJudge)
        {
            return JudgePermissions;
        }
        return [];
    }

    private async ValueTask<JudgementFact> GetJudgementFactAsync(Guid judgeeUserId, CancellationToken cancellationToken)
    {
        var fact = _authorizationContext.Get<JudgementFact>().FirstOrDefault(f => f.JudgeeId == judgeeUserId);
        if (fact != null)
        {
            return fact;
        }
        fact = new JudgementFact(judgeeUserId, CanJudge: true);
        _authorizationContext.Store(fact);
        return fact;
    }
}

public enum JudgementPermission
{
    Judge
}