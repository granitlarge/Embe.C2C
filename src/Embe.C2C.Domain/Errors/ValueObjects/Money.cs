namespace Embe.C2C.Domain.Errors.ValueObjects;

public static class MoneyErrors
{
    public static readonly DomainError Negative = new("money.negative", "A 'money' amount must greater than or equal to 0.");
}