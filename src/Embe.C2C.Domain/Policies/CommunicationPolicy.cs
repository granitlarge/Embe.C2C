using Embe.C2C.Domain.Aggregates.Blockings;
using Embe.C2C.Domain.Aggregates.Contacts;
using Embe.C2C.Domain.Aggregates.Users;

namespace Embe.C2C.Domain.Policies;

public record CommunicationPolicy
(
    User Sender,
    User Receiver,
    Contact? Contact = null,
    Blocking? Blocking1 = null,
    Blocking? Blocking2 = null
)
{
    public bool CanCommunicate()
    {
        var isContact = Contact != null;
        var senderBlocking = Blocking1?.BlockerUserId == Sender.Id ? Blocking1 : Blocking2?.BlockerUserId == Sender.Id ? Blocking2 : throw new InvalidOperationException("Neither blocking belongs to the sender.");
        var receiverBlocking = Blocking1?.BlockerUserId == Receiver.Id ? Blocking1 : Blocking2?.BlockerUserId == Receiver.Id ? Blocking2 : throw new InvalidOperationException("Neither blocking belongs to the receiver.");
        return isContact && senderBlocking == null && receiverBlocking == null;
    }
}