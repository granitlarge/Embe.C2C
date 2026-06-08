using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Domain.Entities;

public class File : Entity
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

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private File()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {

    }

    public Guid Id { get; }
    public Guid OwnerUserId { get; }
    public FileDetails FileDetails { get; private set; }
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

    internal void ChangeOrder(int newOrder)
    {
        FileDetails = FileDetails with { Order = newOrder };
    }

    internal static File Create(Guid ownerUserId, FileDetails fileDetails)
    {
        return new File(ownerUserId, fileDetails);
    }

}