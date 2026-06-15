using Embe.C2C.Domain.Aggregates.Blockings;
using Embe.C2C.Domain.Aggregates.Contacts;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Users;

namespace Embe.C2C.Domain.Policies;

public record CommunicationPolicy
(
    User Sender,
    User Receiver,
    Matching? Matching = null,
    Blocking? Blocking1 = null,
    Blocking? Blocking2 = null
)
{
    public bool CanCommunicate()
    {
        var isMatching = Matching != null;

        var senderBlocking = 
            Blocking1?.BlockerUserId == Sender.Id ? Blocking1 : 
            Blocking2?.BlockerUserId == Sender.Id ? Blocking2 : 
            Blocking1 != null || Blocking2 != null ? throw new InvalidOperationException("Neither blocking belongs to the sender.") : 
            null;

        var receiverBlocking =
            Blocking1?.BlockerUserId == Receiver.Id ? Blocking1 :
            Blocking2?.BlockerUserId == Receiver.Id ? Blocking2 :
            Blocking1 != null || Blocking2 != null ? throw new InvalidOperationException("Neither blocking belongs to the receiver.") :
            null;

        return isMatching && senderBlocking == null && receiverBlocking == null;
    }
}