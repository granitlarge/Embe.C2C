using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.ValueObjects;

public record ImageDetails
{
    public ImageDetails(string name, string mimeType, int order, ImageStatus status)
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
        Status = status;
    }

    public string Name { get; }
    public string MimeType { get; }
    public int Order { get; init; }
    public ImageStatus Status { get; init; }
}

public enum ImageDetailsError
{
    InvalidOrder,
    InvalidName,
    InvalidMimeType
}

public enum ImageStatus
{
    Pending = 1,
    Rejected = 2,
    Accepted = 3
}