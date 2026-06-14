using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Dtos.Read.ValueObjects;

public record MoneyDto
(
    decimal Amount,
    CurrencyDto Currency
);

public static class MoneyDtoExtensions
{
    public static MoneyDto ToDto(this Money money)
    {
        return new MoneyDto(money.Amount, money.Currency.ToDto());
    }
}
