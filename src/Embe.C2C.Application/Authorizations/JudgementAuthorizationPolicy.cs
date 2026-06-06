using System.Collections.Immutable;

namespace Embe.C2C.Application.Authorizations;

public class JudgementAuthorizationPolicy
{
    public JudgementAuthorizationPolicy()
    {

    }

    public Task<ImmutableHashSet<JudgementPermission>> GetPermissionsAsync
    (
        Guid targetJudgementId, 
        CancellationToken cancellationToken = default
    )
    {
        var permissions = new HashSet<JudgementPermission>()
        {
            JudgementPermission.Judge
        }.ToImmutableHashSet();
        return Task.FromResult(permissions);
    }

}

public enum JudgementPermission
{
    Judge
}