using Embe.C2C.Application.Dtos.Read.ValueObjects;

namespace Embe.C2C.Application.Dtos.Write.Aggregates;

public record AccountWriteDto
(
    Guid Id,
    Guid UserId,
    MoneyDto Balance,
    bool IsOpen
);