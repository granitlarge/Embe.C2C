using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.ValueObjects;

public record FileDetails
{
    public FileDetails(string url, string mimeType, int order)
    {
        if (order < 0)
        {
            throw new DomainException("Order must be a non-negative integer.");
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            throw new DomainException("URL cannot be null or whitespace.");
        }

        if (string.IsNullOrWhiteSpace(mimeType))
        {
            throw new DomainException("MIME type cannot be null or whitespace.");
        }

        Url = url;
        MimeType = mimeType;
        Order = order;
    }

    public string Url { get; }
    public string MimeType { get; }
    public int Order { get; init;}
}