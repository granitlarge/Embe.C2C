namespace Embe.C2C.Domain.Errors.Aggregates;

public static class AccountErrors
{
    public static readonly DomainError AccountCloseAlreadyClosed = new("account.close_already_closed", "Cannot close an already closed account.");
    public static readonly DomainError AccountClosePositiveBalance = new("account.close_positive_balance", "Cannot close an account with a positive balance.");
    public static readonly DomainError AccountOpenAlreadyOpened = new("account.open_already_opened", "Cannot open an account that is already open.");
    public static readonly DomainError AccountRemoveWhileOpen = new("account.remove_open", "Cannot remove an account that is still open.");
    public static readonly DomainError AccountTransactWhileClosed = new("account.transact_closed", "Cannot withdraw from a closed account.");
    public static readonly DomainError AccountTransactIncorrectCurrency = new("account.transact_incorrect_currency", "The currency specified in the requested transaction differs from the currency of the account.");
    public static readonly DomainError AccountTransactNonPositiveAmount = new("account.transact_non_positive_amount", "The transaction request was denied because the requested amount to transact was non-positive.");
    public static readonly DomainError AccountWithdrawExceedsBalance = new("account.withdraw_exceeds_balanace", "The withdrawal request was declined because it exceeds the balance on the account.");
}