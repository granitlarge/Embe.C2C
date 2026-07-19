using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.ValueObjects;

public record ImageDetails
{
    public ImageDetails
    (
        string name, 
        string mimeType, 
        int order, 
        ImageStatus status,
        double cropOffsetX,
        double cropOffsetY
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

        if (cropOffsetX < 0 || cropOffsetY < 0)
        {
            throw new DomainException(new DomainError<ImageDetailsError>(ImageDetailsError.InvalidOffsets));
        }

        Name = name;
        MimeType = mimeType;
        Order = order;
        Status = status;
        CropOffsetX = cropOffsetX;
        CropOffsetY = cropOffsetY;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private ImageDetails()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {

    }

    public string Name { get; }
    public string MimeType { get; }
    public int Order { get; init; }
    public ImageStatus Status { get; init; }
    public double CropOffsetX {get;}
    public double CropOffsetY {get;}
}

public record ImageCropOffset
{
    public ImageCropOffset
    (
        double x,
        double y
    )
    {
        if (x < 0 || y < 0)
            throw new DomainException(new DomainError<ImageCropError>(ImageCropError.NegativeOffset));
        X = x;
        Y = y;
    }

    private ImageCropOffset()
    {

    }

    public double X { get; }
    public double Y { get; }
}

public enum ImageDetailsError
{
    InvalidOrder = 1,
    InvalidName = 2,
    InvalidMimeType = 3,
    InvalidOffsets = 4
}

public enum ImageStatus
{
    Pending = 1,
    Rejected = 2,
    Accepted = 3
}

public enum ImageCropError
{
    NegativeOffset
}