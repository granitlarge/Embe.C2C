using Embe.C2C.Domain.Errors;
using Embe.C2C.Domain.Errors.ValueObjects;
using ErrorOr;

namespace Embe.C2C.Domain.ValueObjects;

public record ImageDetails
{
    private ImageDetails
    (
        string name,
        string mimeType,
        int order
    )
    {
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

    public static ErrorOr<ImageDetails> Create
    (
        string name,
        string mimeType,
        int order
    )
    {
        var errors = new List<Error>();
        if (order < 0)
        {
            errors.Add(ImageDetailsErrors.NegativeOrder.ToValidationErrorOr());
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(DomainErrors.Empty.ToValidationErrorOr());
        }

        if (string.IsNullOrWhiteSpace(mimeType))
        {
            errors.Add(DomainErrors.Empty.ToValidationErrorOr());
        }

        return new ImageDetails(name, mimeType, order);
    }
}