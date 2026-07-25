namespace Embe.C2C.Domain.Errors.Aggregates;

public static class MatchingErrors
{
    public static readonly DomainError SendMessageCannotCommunicate = new("matching.send_message_cannot_communicate", "The author is not allowed to communicate with the recipient.");
}