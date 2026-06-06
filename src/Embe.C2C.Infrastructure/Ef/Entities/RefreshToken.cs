namespace Embe.C2C.Infrastructure.Ef.Entities;

public class RefreshTokenEntity
{
    public RefreshTokenEntity(Guid id, Guid userId, DateTimeOffset expiresAt)
    {
        if (expiresAt <= DateTimeOffset.UtcNow)
            throw new ArgumentException("Expiration time must be in the future.", nameof(expiresAt));
        Id = id;
        UserId = userId;
        ExpiresAt = expiresAt;
    }

    private RefreshTokenEntity()
    {

    }

    public Guid Id { get; }
    public Guid UserId { get; }
    public DateTimeOffset ExpiresAt { get; private set; }

    public void Expire()
    {
        ExpiresAt = DateTimeOffset.UtcNow;
    }
}