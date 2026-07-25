using Embe.C2C.Domain.Aggregates.Blockings;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Exceptions;
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
            return DomainErrors.BlockingAlreadyExists.ToValidationErrorOr(new Dictionary<string, object>
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