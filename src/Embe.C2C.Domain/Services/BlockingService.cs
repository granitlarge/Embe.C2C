using Embe.C2C.Domain.Aggregates.Blockings;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.Services;

public class BlockingService : DomainService
{
    public Blocking Block
    (
        Blocking? existingBlocking,
        User blockingUser,
        User blockedUser
    )
    {
        if (existingBlocking != null)
        {
            throw new DomainException(new DomainError<BlockingServiceError>(BlockingServiceError.AlreadyExists));
        }

        var blocking = Blocking.Create(blockingUser.Id, blockedUser.Id);
        return blocking;
    }
}

public enum BlockingServiceError
{
    AlreadyExists
}