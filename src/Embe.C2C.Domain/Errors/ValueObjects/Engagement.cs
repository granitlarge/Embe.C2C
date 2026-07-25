namespace Embe.C2C.Domain.Errors.ValueObjects;

public static class EngagementErrors
{
    public static readonly DomainError OneTimeEngagementMustHaveOnceFrequency = new("engagement.one_time_engagement_must_have_once_frequency", "A one-time engagement must have a frequency of 'once'.");
    public static readonly DomainError FixedTermRequiresStartAndEndDate = new("engagement.fixed_term_requires_start_and_end_date", "A fixed-term engagement must have both a start date and an end date.");
    public static readonly DomainError FixedTermStartDateAfterEndDate = new("engagement.fixed_term_start_date_after_end_date", "A fixed-term engagement must have a start date that is before or equal to the end date.");
    public static readonly DomainError StartDateAndEndDateMustBeNullForNonFixedTerm = new("engagement.start_date_and_end_date_must_be_null_for_non_fixed_term", "A non-fixed-term engagement must have both a null start date and a null end date.");
}