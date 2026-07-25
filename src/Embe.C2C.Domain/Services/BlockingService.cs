using Embe.C2C.Domain.Aggregates.Blockings;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Errors;
using Embe.C2C.Domain.Errors.Aggregates;
using ErrorOr;

namespace Embe.C2C.Domain.Services;

public class BlockingService : DomainService
{
    public ErrorOr<Blocking> Block
    (
        Blocking? existingBlocking,
        User blockingUser,
        User blockedUser
    )
    {
        if (existingBlocking != null)
        {
            return BlockingErrors.AlreadyExists.ToRuleErrorOr(new Dictionary<string, object>
            {
                { "blockingUserId", blockingUser.Id },
                { "blockedUserId", blockedUser.Id }
            });
        }

        var blocking = Blocking.Create(blockingUser.Id, blockedUser.Id);
        return blocking;
    }
}

public enum BlockingServiceError
{
    AlreadyExists
}