namespace Embe.C2C.Application.Dtos.ValueObjects;

public record CurrencyDto
(
    string Code,
    string Name,
    string Symbol
);

public static class CurrencyDtoExtensions
{
    public static CurrencyDto ToDto(this Domain.ValueObjects.Currency currency)
    {
        return new CurrencyDto(currency.Code, currency.Name, currency.Symbol);
    }
}
