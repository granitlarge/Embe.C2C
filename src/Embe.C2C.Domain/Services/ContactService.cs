using Embe.C2C.Domain.Aggregates.Blockings;
using Embe.C2C.Domain.Aggregates.ContactRequests;
using Embe.C2C.Domain.Aggregates.Contacts;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.Services;

public class ContactService : DomainService
{
    public ContactRequest SendContactRequest
    (
        ContactRequest? existingContactRequest,
        Contact? existingContact,
        Blocking? blocking,
        User requester,
        User recipient
    )
    {
        if (existingContactRequest != null)
        {
            throw new DomainException(new DomainError<ContactRequestError>(ContactRequestError.AlreadyExists));
        }

        if (existingContact != null)
        {
            throw new DomainException(new DomainError<ContactRequestError>(ContactRequestError.AlreadyExists));
        }

        if (blocking != null)
        {
            throw new DomainException(new DomainError<ContactRequestError>(ContactRequestError.BlockingExists));
        }

        if (requester.Id == recipient.Id)
        {
            throw new DomainException(new DomainError<ContactRequestError>(ContactRequestError.SelfRequest));
        }

        var contactRequest = ContactRequest.Create(requester.Id, recipient.Id);
        return contactRequest;
    }

    public Contact AcceptContactRequest
    (
        ContactRequest contactRequest,
        Blocking? blocking
    )
    {
        if (blocking != null)
        {
            throw new DomainException(new DomainError<ContactRequestError>(ContactRequestError.BlockingExists));
        }

        var contact = contactRequest.Accept();
        return contact;
    }
}