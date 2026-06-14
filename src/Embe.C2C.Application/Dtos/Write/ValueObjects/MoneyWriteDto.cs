namespace Embe.C2C.Application.Dtos.Write.ValueObjects;

public record MoneyWriteDto
(
    decimal Amount,
    CurrencyWriteDto Currency
);