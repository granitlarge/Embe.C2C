using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.ValueObjects;

public record ImageDetails
{
    public ImageDetails
    (
        string name,
        string mimeType,
        int order
    )
    {
        if (order < 0)
        {
            throw new DomainException(new DomainError<ImageDetailsError>(ImageDetailsError.InvalidOrder));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(new DomainError<ImageDetailsError>(ImageDetailsError.InvalidName));
        }

        if (string.IsNullOrWhiteSpace(mimeType))
        {
            throw new DomainException(new DomainError<ImageDetailsError>(ImageDetailsError.InvalidMimeType));
        }

        Name = name;
        MimeType = mimeType;
        Order = order;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private ImageDetails()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {

    }

    public string Name { get; }
    public string MimeType { get; }
    public int Order { get; init; }
}

public enum ImageDetailsError
{
    InvalidOrder = 1,
    InvalidName = 2,
    InvalidMimeType = 3,
    InvalidOffsets = 4
}