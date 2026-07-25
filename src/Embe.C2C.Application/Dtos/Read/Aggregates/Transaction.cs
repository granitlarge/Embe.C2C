using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Dtos.Read.ValueObjects;
using Embe.C2C.Domain.Aggregates.Transactions;
using Embe.C2C.Domain.Errors.Aggregates;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Dtos.Read.Aggregates;

public record TransactionDto
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

public static class TransactionDtoExtensions
{
    public static TransactionDto ToDto(this Transaction transaction)
    {
        return new TransactionDto
        (
            transaction.Id,
            transaction.AccountId,
            transaction.Amount.ToDto(),
            transaction.Type,
            transaction.Reason,
            transaction.TransactionDate,
            transaction.Note,
            transaction.CreatedAt
        );
    }
}
