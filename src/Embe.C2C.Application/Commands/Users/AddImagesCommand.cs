namespace Embe.C2C.Application.Commands.Users;

public record ImageWriteDto
(
    string Base64EncodedImageData,
    string MimeType,
    int Order,
    double CropOffsetX,
    double CropOffsetY,
    double Width,
    double Height
);

public record AddImagesCommand(ImageWriteDto[] Images);