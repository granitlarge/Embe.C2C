namespace Embe.C2C.Application.Dtos.Write.Aggregates;

public record ContactRequestWriteDto
(
    Guid Id,
    Guid RequestorUserId,
    Guid RecipientUserId,
    bool? IsAccepted,
    DateTimeOffset? RespondedAt,
    DateTimeOffset RequestedAt
);