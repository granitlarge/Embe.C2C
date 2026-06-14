using Embe.C2C.Application.Dtos.Read.ValueObjects;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Dtos.Write.Aggregates;

public record TransactionWriteDto
(
    Guid Id,
    Guid AccountId,
    MoneyDto Amount,
    TransactionType Type,
    TransactionReason Reason,
    DateTimeOffset TransactionDate,
    string? Note,
    DateTimeOffset CreatedAt
);
