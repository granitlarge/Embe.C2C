namespace Embe.C2C.Domain.Entities;

public abstract class Entity
{
    public byte[] RowVersion { get; private set; } = null!;
}