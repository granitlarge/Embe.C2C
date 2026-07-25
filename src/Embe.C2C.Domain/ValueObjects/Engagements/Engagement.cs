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
            return DomainErrors.EngagementOneTimeEngagementMustHaveOnceFrequency.ToValidationErrorOr();
        }

        if (boundedness == EngagementBoundedness.FixedTerm)
        {
            if (startDate is null || endDate is null)
            {
                return DomainErrors.EngagementFixedTermRequiresStartAndEndDate.ToValidationErrorOr();
            }

            if (startDate > endDate)
            {
                return DomainErrors.EngagementFixedTermStartDateAfterEndDate.ToValidationErrorOr();
            }
        }
        else
        {
            if (startDate is not null || endDate is not null)
            {
                return DomainErrors.EngagementStartDateAndEndDateMustBeNullForNonFixedTerm.ToValidationErrorOr();
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