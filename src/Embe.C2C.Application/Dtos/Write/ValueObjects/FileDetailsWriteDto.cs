namespace Embe.C2C.Application.Dtos.Write.ValueObjects;

public record FileDetailsWriteDto
(
    string Url,
    string? Name,
    string MimeType,
    int? Order
);