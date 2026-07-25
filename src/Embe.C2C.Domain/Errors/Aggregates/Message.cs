namespace Embe.C2C.Domain.Errors.Aggregates;

public static class MessageErrors
{
    public static readonly DomainError InvalidReply = new("message.invalid_reply", "The specified message is not a valid reply.");
}