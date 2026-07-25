using Embe.C2C.Domain.Errors;
using Embe.C2C.Domain.Errors.ValueObjects;
using Embe.C2C.Domain.ValueObjects.Engagements.Enums;
using ErrorOr;

namespace Embe.C2C.Domain.ValueObjects.Engagements;

public record Engagement
{
    public static ErrorOr<Engagement> Create
    (
        EngagementMedium medium,
        EngagementBoundedness boundedness,
        EngagementFrequency frequency,
        DateOnly? startDate,
        DateOnly? endDate
    )
    {

        if (
            boundedness == EngagementBoundedness.OneTime && frequency != EngagementFrequency.Once ||
            boundedness != EngagementBoundedness.OneTime && frequency == EngagementFrequency.Once
        )
        {
            return EngagementErrors.OneTimeEngagementMustHaveOnceFrequency.ToValidationErrorOr();
        }

        if (boundedness == EngagementBoundedness.FixedTerm)
        {
            if (startDate is null || endDate is null)
            {
                return EngagementErrors.FixedTermRequiresStartAndEndDate.ToValidationErrorOr();
            }

            if (startDate > endDate)
            {
                return EngagementErrors.FixedTermStartDateAfterEndDate.ToValidationErrorOr();
            }
        }
        else
        {
            if (startDate is not null || endDate is not null)
            {
                return EngagementErrors.StartDateAndEndDateMustBeNullForNonFixedTerm.ToValidationErrorOr();
            }
        }

        return new Engagement(medium, boundedness, frequency, startDate, endDate);

    }

    private Engagement
    (
        EngagementMedium medium,
        EngagementBoundedness boundedness,
        EngagementFrequency frequency,
        DateOnly? startDate,
        DateOnly? endDate
    )
    {
        Medium = medium;
        Boundedness = boundedness;
        Frequency = frequency;
        StartDate = startDate;
        EndDate = endDate;
    }

    private Engagement()
    {

    }

    public EngagementMedium Medium { get; private set; }
    public EngagementBoundedness Boundedness { get; private set; }
    public EngagementFrequency Frequency { get; private set; }
    public DateOnly? StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
}