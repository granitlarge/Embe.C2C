namespace Embe.C2C.Application.Commands.Images;

public record ProcessUploadedImageCommand
(
    string ImageName,
    byte[] ImageBytes
);