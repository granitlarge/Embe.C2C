using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Domain.Entities;

public class Image : Entity
{
    private Image
    (
        Guid ownerUserId,
        ImageDetails imageDetails
    )
    {
        Id = Guid.CreateVersion7();
        OwnerUserId = ownerUserId;
        ImageDetails = imageDetails;
        MarkedForDeletionAt = null;
        DeletedAt = null;
        CreatedAt = DateTimeOffset.UtcNow;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Image()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {

    }

    public Guid Id { get; }
    public Guid OwnerUserId { get; }
    public ImageDetails ImageDetails { get; private set; }
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
        ImageDetails = ImageDetails with { Order = newOrder };
    }

    internal static Image Create(Guid ownerUserId, ImageDetails imageDetails)
    {
        return new Image(ownerUserId, imageDetails);
    }

}