namespace Embe.C2C.Application.Extensions;

public static class StringExtensions
{
    public static string ToDataUrl(this byte[] data, string mimeType)
    {
        var base64 = Convert.ToBase64String(data);
        return $"data:{mimeType};base64,{base64}";
    }

    public static byte[] FromDataUrl(this string dataUrl)
    {
        if (!dataUrl.StartsWith("data:"))
        {
            throw new ArgumentException("Invalid data URL format.", nameof(dataUrl));
        }
        if (!dataUrl.Contains(";base64,"))
        {
            throw new ArgumentException("Data URL does not contain base64 data.", nameof(dataUrl));
        }
        if (dataUrl.IndexOf(";base64,") < 0)
        {
            throw new ArgumentException("Data URL does not contain base64 data.", nameof(dataUrl));
        }
        if (dataUrl.IndexOf(',') < 0)
        {
            throw new ArgumentException("Data URL does not contain base64 data.", nameof(dataUrl));
        }
        var base64 = dataUrl[(dataUrl.IndexOf(',') + 1)..];
        return Convert.FromBase64String(base64);
    }
}