namespace Embe.C2C.Domain.ValueObjects
{
    public enum Role
    {
        User = 0,
    }

    public record Actor
    (
        Guid Id,
        Role Role
    );
}