namespace Embe.C2C.Domain.Errors.Aggregates;

public static class TransactionErrors
{
    public static readonly DomainError AmountInvalid = new("transaction.amount_invalid", "The specified transaction amount is invalid.");
    public static readonly DomainError FutureDate = new("transaction.future_date", "The specified transaction date is in the future.");
}