using System.ComponentModel.DataAnnotations;

namespace Embe.C2C.Domain.Entities;

public abstract class Entity
{
    [Timestamp]
    public uint RowVersion { get; private set; }
}