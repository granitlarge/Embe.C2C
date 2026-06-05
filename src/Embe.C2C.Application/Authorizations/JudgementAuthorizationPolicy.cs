using System.Collections.Immutable;

namespace Embe.C2C.Application.Authorizations;

public class JudgementAuthorizationPolicy
{
    public JudgementAuthorizationPolicy()
    {

    }

    public async Task<ImmutableHashSet<JudgementPermission>> GetPermissionsAsync
    (
        Guid targetJudgementId, 
        CancellationToken cancellationToken = default
    )
    {
        var permissions = new HashSet<JudgementPermission>()
        {
            JudgementPermission.Judge
        };
        return [.. permissions];
    }

}

public enum JudgementPermission
{
    Judge
}