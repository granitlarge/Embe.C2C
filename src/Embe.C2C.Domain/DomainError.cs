using ErrorOr;

namespace Embe.C2C.Domain;

public record DomainError(string Group, object Value);
public record DomainError<T>(T ErrorCode) : DomainError(typeof(T).Name, ErrorCode) where T : Enum;
public record DomainErrorNew(string Code, string Message);

public static class DomainErrors
{
    public static readonly DomainErrorNew Empty = new("empty", "A value must be provided, but none was.");
    public static readonly DomainErrorNew NegativeMoney = new("money.negative", "A 'money' amount must greater than or equal to 0.");
    public static readonly DomainErrorNew InvalidLatitude = new("location.invalid_latitude", "Latitude must be between -90 and 90.");
    public static readonly DomainErrorNew InvalidLongitude = new("location.invalid_longitude", "Longitude must be between -180 and 180.");
    public static readonly DomainErrorNew NegativeOrder = new("image.order_negative", "Order must be greater than or equal to 0.");
    public static readonly DomainErrorNew InvalidEmail = new("email.invalid", "The e-mail provided is invalid.");
    public static readonly DomainErrorNew NegativeDistance = new("distance.negative", "Distance cannot be negative");
    public static readonly DomainErrorNew InvalidBirthdate = new("birthdate.invalid", "Birthdate is invalid");
    public static readonly DomainErrorNew AgeOutOfRange = new("age.out_of_range", "Age must be between 0 and 120");

    public static readonly DomainErrorNew AccountCloseAlreadyClosed = new("account.close_already_closed", "Cannot close an already closed account.");
    public static readonly DomainErrorNew AccountClosePositiveBalance = new("account.close_positive_balance", "Cannot close an account with a positive balance.");
    public static readonly DomainErrorNew AccountOpenAlreadyOpened = new("account.open_already_opened", "Cannot open an account that is already open.");
    public static readonly DomainErrorNew AccountRemoveWhileOpen = new("account.remove_open", "Cannot remove an account that is still open.");
    public static readonly DomainErrorNew AccountTransactWhileClosed = new("account.transact_closed", "Cannot withdraw from a closed account.");
    public static readonly DomainErrorNew AccountTransactIncorrectCurrency = new("account.transact_incorrect_currency", "The currency specified in the requested transaction differs from the currency of the account.");
    public static readonly DomainErrorNew AccountTransactNonPositiveAmount = new("account.transact_non_positive_amount", "The transaction request was denied because the requested amount to transact was non-positive.");
    public static readonly DomainErrorNew AccountWithdrawExceedsBalance = new("account.withdraw_exceeds_balanace", "The withdrawal request was declined because it exceeds the balance on the account.");

    public static readonly DomainErrorNew UserSame = new("user.same", "The same user cannot be specified as both arguments.");

    public static readonly DomainErrorNew Forbidden = new("forbidden", "The actor that attempted to perform this operation is forbidden to do so.");

    public static readonly DomainErrorNew SearchProfileGendersEmpty = new("search_profile.genders_empty", "At least 1 gender must be specified in a search profile");
    public static readonly DomainErrorNew SearchProfileAgeRangeInvalid = new("search_profile.age_range_invalid", "The specified age range is invalid.");
    public static readonly DomainErrorNew SearchProfileGendersInvalid = new("search_profile.genders_invalid", "The specified genders are invalid.");
    public static readonly DomainErrorNew SearchProfileOwnerDistanceFilterButLocationNotSet = new("search_profile.owner_distance_filter_but_location_not_set", "The search profile owner has specified a distance filter, but their location is not set. The owner must set their location before specifying a distance filter.");

    public static readonly DomainErrorNew TransactionAmountInvalid = new("transaction.amount_invalid", "The specified transaction amount is invalid.");
    public static readonly DomainErrorNew TransactionFutureDate = new("transaction.future_date", "The specified transaction date is in the future.");

    public static readonly DomainErrorNew UserAgeOutOfRange = new("user.age_out_of_range", "The specified user age must be between 18 and 120.");
    public static readonly DomainErrorNew UserInvalidFileCount = new("user.invalid_file_count", "The specified user has an invalid number of files.");

    public static readonly DomainErrorNew BlockingAlreadyExists = new("blocking.already_exists", "The specified blocking already exists.");

    public static readonly DomainErrorNew MatchingSendMessageCannotCommunicate = new("matching.send_message_cannot_communicate", "The author is not allowed to communicate with the recipient.");

    public static readonly DomainErrorNew MessageInvalidReply = new("message.invalid_reply", "The specified message is not a valid reply.");

    public static readonly DomainErrorNew EngagementOneTimeEngagementMustHaveOnceFrequency = new("engagement.one_time_engagement_must_have_once_frequency", "A one-time engagement must have a frequency of 'once'.");
    public static readonly DomainErrorNew EngagementFixedTermRequiresStartAndEndDate = new("engagement.fixed_term_requires_start_and_end_date", "A fixed-term engagement must have both a start date and an end date.");
    public static readonly DomainErrorNew EngagementFixedTermStartDateAfterEndDate = new("engagement.fixed_term_start_date_after_end_date", "A fixed-term engagement must have a start date that is before or equal to the end date.");
    public static readonly DomainErrorNew EngagementStartDateAndEndDateMustBeNullForNonFixedTerm = new("engagement.start_date_and_end_date_must_be_null_for_non_fixed_term", "A non-fixed-term engagement must have both a null start date and a null end date.");
}

public static class DomainErrorNewExtensions
{
    public static Error ToValidationErrorOr(this DomainErrorNew domainErrorNew, Dictionary<string, object>? metadata = null)
    {
        return Error.Validation(domainErrorNew.Code, domainErrorNew.Message, metadata);
    }

    public static Error ToFailureErrorOr(this DomainErrorNew domainErrorNew, Dictionary<string, object>? metadata = null)
    {
        return Error.Failure(domainErrorNew.Code, domainErrorNew.Message, metadata);
    }

    public static Error ToForbiddenErrorOr(this DomainErrorNew domainErrorNew, Dictionary<string, object>? metadata = null)
    {
        return Error.Forbidden(domainErrorNew.Code, domainErrorNew.Message, metadata);
    }
}