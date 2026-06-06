using Embe.C2C.Application.Dtos.ValueObjects;
using Embe.C2C.Domain.Aggregates.Accounts;

namespace Embe.C2C.Application.Dtos.Aggregates;

public record AccountDto
(
    Guid Id,
    Guid UserId,
    MoneyDto Balance,
    bool IsOpen
);

public static class AccountDtoExtensions
{
    public static AccountDto ToDto(this Account account)
    {
        return new AccountDto
        (
            account.Id,
            account.UserId,
            account.Balance.ToDto(),
            account.IsOpen
        );
    }
}
