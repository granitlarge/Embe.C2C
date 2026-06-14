namespace Embe.C2C.Application.Dtos.Write.Aggregates;

public record ContactWriteDto
(
    Guid Id,
    Guid UserId1,
    Guid UserId2,
    DateTimeOffset CreatedAt
);