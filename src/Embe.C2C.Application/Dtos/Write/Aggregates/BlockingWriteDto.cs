namespace Embe.C2C.Application.Dtos.Write.Aggregates;

public record BlockingWriteDto
(
    Guid Id,
    Guid BlockerUserId,
    Guid BlockedUserId,
    DateTimeOffset BlockedAt
);