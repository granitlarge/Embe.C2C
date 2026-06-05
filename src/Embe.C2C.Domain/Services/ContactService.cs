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
            throw new DomainException("A contact request already exists between the requester and recipient.");
        }

        if (existingContact != null)
        {
            throw new DomainException("A contact already exists between the requester and recipient.");
        }

        if (blocking != null)
        {
            throw new DomainException("A blocking exists between the requester and recipient, so a contact request cannot be sent.");
        }

        if (requester.Id == recipient.Id)
        {
            throw new DomainException("A user cannot send a contact request to themselves.");
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
            throw new DomainException("A blocking exists between the requester and recipient, so the contact request cannot be accepted.");
        }

        var contact = contactRequest.Accept();
        return contact;
    }
}