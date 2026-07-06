using System.Collections.Immutable;

namespace Embe.C2C.Application.Commands.Users;

public record UpdateImagesCommand
(
    ImmutableHashSet<UpdateFileDto> FilesToKeep,
    ImmutableHashSet<CreateFileDto> FilesToAdd
);
public record UpdateFileDto
(
    Guid Id,
    int Order
);