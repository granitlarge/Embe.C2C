namespace Embe.C2C.Domain.ValueObjects.Engagements.Enums;

public enum EngagementDomainError
{
    OneTimeEngagementMustHaveOnceFrequency,
    FixedTermStartDateAfterEndDate,
    FixedTermRequiresStartAndEndDate,
    StartDateAndEndDateMustBeNullForNonFixedTerm
}