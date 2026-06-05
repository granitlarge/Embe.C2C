namespace Embe.C2C.Domain.ValueObjects;

public record FileDetails
{
    public FileDetails(string url, string mimeType)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("URL cannot be null or whitespace.", nameof(url));
        }

        if (string.IsNullOrWhiteSpace(mimeType))
        {
            throw new ArgumentException("MIME type cannot be null or whitespace.", nameof(mimeType));
        }

        Url = url;
        MimeType = mimeType;
    }

    public string Url { get; }
    public string MimeType { get; }
}