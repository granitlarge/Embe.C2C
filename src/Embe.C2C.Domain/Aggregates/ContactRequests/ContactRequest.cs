using Embe.C2C.Domain.Aggregates.ContactRequests.Events;
using Embe.C2C.Domain.Aggregates.Contacts;
using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.Aggregates.ContactRequests;

public class ContactRequest : Aggregate
{
    private ContactRequest
    (
        Guid requestorUserId,
        Guid recipientUserId
    )
    {
        if (requestorUserId == recipientUserId)
        {
            throw new DomainException(new DomainError<ContactRequestError>(ContactRequestError.SelfRequest));
        }

        Id = Guid.CreateVersion7();
        RequestorUserId = requestorUserId;
        RecipientUserId = recipientUserId;
        RequestedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new ContactRequestCreatedEvent(this));
    }

    private ContactRequest() { }

    public Guid Id { get; }
    public Guid RequestorUserId { get; }
    public Guid RecipientUserId { get; }
    public bool? IsAccepted { get; private set; }

    public DateTimeOffset? RespondedAt { get; private set; }
    public DateTimeOffset RequestedAt { get; private set;}

    internal Contact Accept()
    {
        if (IsAccepted.HasValue)
        {
            throw new DomainException(new DomainError<ContactRequestError>(ContactRequestError.AlreadyResponded));
        }

        IsAccepted = true;
        RespondedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new ContactRequestAcceptedEvent(this));
        return Contact.Create(RequestorUserId, RecipientUserId);
    }

    public void Reject()
    {
        if (IsAccepted.HasValue)
        {
            throw new DomainException(new DomainError<ContactRequestError>(ContactRequestError.AlreadyResponded));
        }

        IsAccepted = false;
        RespondedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new ContactRequestRejectedEvent(this));
    }

    public void Remove()
    {
        AddDomainEvent(new ContactRequestRemovedEvent(this));
    }

    internal static ContactRequest Create(Guid requestorUserId, Guid recipientUserId)
    {
        return new ContactRequest(requestorUserId, recipientUserId);
    }
}

public enum ContactRequestError
{
    SelfRequest,
    AlreadyResponded,
    AlreadyExists,
    BlockingExists
}