namespace Embe.C2C.Application.Commands.Images;

public record ProcessUploadedImageCommand
(
    Guid ImageId,
    byte[] ImageBytes
);