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
            throw new DomainException("A blocking already exists between the blocker and blocked users.");
        }

        var blocking = Blocking.Create(blockingUser.Id, blockedUser.Id);
        return blocking;
    }
}