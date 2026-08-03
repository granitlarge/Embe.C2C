using System.Collections.Immutable;
using Embe.C2C.Domain.Aggregates.Accounts;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Aggregates.Users.Events;
using Embe.C2C.Domain.Errors.Aggregates;

namespace Embe.C2C.Domain.Services;

public class UserService : DomainService
{
    private readonly DomainEventStore _domainEventStore;

    public UserService(DomainEventStore domainEventStore)
    {
        _domainEventStore = domainEventStore;
    }

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
        _domainEventStore.AddDomainEvent(new UserDeletedEvent(user));
    }
}