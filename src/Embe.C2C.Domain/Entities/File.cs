using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Domain.Entities;

public class File
{
    private File
    (
        Guid ownerUserId,
        FileDetails fileDetails
    )
    {
        Id = Guid.CreateVersion7();
        OwnerUserId = ownerUserId;
        FileDetails = fileDetails;
        MarkedForDeletionAt = null;
        DeletedAt = null;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; }
    public Guid OwnerUserId { get; }
    public FileDetails FileDetails { get; }
    public DateTimeOffset? MarkedForDeletionAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    internal void MarkForDeletion()
    {
        MarkedForDeletionAt = DateTimeOffset.UtcNow;
    }

    internal void MarkAsDeleted()
    {
        DeletedAt = DateTimeOffset.UtcNow;
    }

    internal static File Create(Guid ownerUserId, FileDetails fileDetails)
    {
        return new File(ownerUserId, fileDetails);
    }
}