namespace Embe.C2C.Domain.Errors.Aggregates;

public static class BlockingErrors
{
    public static readonly DomainError AlreadyExists = new("blocking.already_exists", "The specified blocking already exists.");
}