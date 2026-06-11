using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.ValueObjects;

public record FileDetails
{
    public FileDetails(string name, string mimeType, int order)
    {
        if (order < 0)
        {
            throw new DomainException(new DomainError<FileDetailsError>(FileDetailsError.InvalidOrder));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(new DomainError<FileDetailsError>(FileDetailsError.InvalidName));
        }

        if (string.IsNullOrWhiteSpace(mimeType))
        {
            throw new DomainException(new DomainError<FileDetailsError>(FileDetailsError.InvalidMimeType));
        }

        Name = name;
        MimeType = mimeType;
        Order = order;
    }

    public string Name { get; }
    public string MimeType { get; }
    public int Order { get; init; }
}

public enum FileDetailsError
{
    InvalidOrder,
    InvalidName,
    InvalidMimeType
}