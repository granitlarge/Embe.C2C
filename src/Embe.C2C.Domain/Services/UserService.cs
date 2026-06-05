using System.Collections.Immutable;
using Embe.C2C.Domain.Aggregates.Accounts;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Aggregates.Users.Events;

namespace Embe.C2C.Domain.Services;

public class UserService : DomainService
{
    public void Delete
    (
        User user,
        ImmutableHashSet<Account> accounts
    )
    {
        foreach (var account in accounts)
        {
            if (account.IsOpen)
                account.Close();
            account.Remove();
        }

        user.Remove();
        AddDomainEvent(new UserDeletedEvent(user));
    }
}