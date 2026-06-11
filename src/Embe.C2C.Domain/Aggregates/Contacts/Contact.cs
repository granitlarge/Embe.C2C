using Embe.C2C.Domain.Aggregates.Contacts.Events;
using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.Aggregates.Contacts;

public class Contact : Aggregate
{
    private Contact
    (
        Guid userId1,
        Guid userId2
    )
    {
        Id = Guid.CreateVersion7();
        UserId1 = userId1;
        UserId2 = userId2;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    private Contact() { }

    public Guid Id { get; }
    public Guid UserId1 { get; }
    public Guid UserId2 { get; }

    public DateTimeOffset CreatedAt { get; private set;}

    public void Remove(Guid removerId)
    {
        if (removerId != UserId1 && removerId != UserId2)
        {
            throw new DomainException(new DomainError<ContactError>(ContactError.Unauthorized));
        }

        AddDomainEvent(new ContactRemovedEvent(removerId, this));
    }

    internal static Contact Create(Guid userId1, Guid userId2)
    {
        return new Contact(userId1, userId2);
    }
}

public enum ContactError
{
    SelfContact,
    Unauthorized,
    AlreadyExists
}